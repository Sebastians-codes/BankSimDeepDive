namespace CashRegister;

/*
    Modellerar det som behövs för att representera den svenska valutan
    samt metoder för öres avrunding och konvertering till fysisk valuta.
*/
public class SwedishCurrencyModel : CurrencyModel
{
    public SwedishCurrencyModel()
    {
        _suffix = "Kr";
        Add("Tusen", 1000m);
        Add("Femhundra", 500m);
        Add("Tvåhundra", 200m);
        Add("Hundra", 100m);
        Add("Femtio", 50m);
        Add("Tjugo", 20m);
        Add("Tio", 10m);
        Add("Fem", 5m);
        Add("Två", 2m);
        Add("En", 1m);
    }

    /*
       tar in den totala summan som ska ges tillbaka.

       skapar en Dictionary i metoden för att hålla namnet på valutan och antalet gånger som
       man kan dela med värdet i ett KeyValuePair.

       skapar en variabel för att inte modifiera parametern
       och assignar värdet efter kör summan genom öresavrundnings metoden.

       använder mig av en foreach loop för att det är väldigt lätt att jobba
       med IEnumerable's med en sådan.

       skapar en temp variable som håller antalet gånger som total summan kan delas
       med summan från val.Value.

       subtraherar temp * val.Value för att få det värdet som är kvar efter delning.
       omvandlar även temp till en int för att avrunda talet neråt innan det gångras
       med värdet av valutan.

       om värdet på temp är mer än 1 så adderar vi ett Kvp till Dictionary med key
       val.Name och värdet val.Value.
       omvändlar även temp till int i både checken och vid addering till dictionary.
       för att få korrekta resultat.

       Formatstrings tar dictionaryt och omvandlar det till en Lista med strings.

    */
    public override void ConvertToPhysical(decimal amount)
    {
        Dictionary<string, decimal> amountOfBillsNCoins = [];
        decimal amountPayed = CheckÖresAvrundning(amount);
        foreach (var val in currencies)
        {
            decimal temp = amountPayed / val.Value;
            amountPayed -= (int)temp * val.Value;
            if ((int)temp > 0)
                amountOfBillsNCoins[val.Name] = (int)temp;
        }
        FormatStrings(amountOfBillsNCoins);
    }

    /*
        kollar hur stort decimal talet är genom att % 1.
        om resultatet är mer än 0.24 och mindre än 0.76 så retureras decimalen avrundad till 0.50
        om det är mer än 0.75 så tar decimalen bort och 1 läggs till i totalen.
        annars så tas decimalen bort och vi returerar värdet utan decimalen.
    */
    private decimal CheckÖresAvrundning(decimal amount)
    {
        decimal ören = amount % 1;
        if (ören > 0.50m)
            return amount - ören + 1m;
        else
            return amount - ören;
    }

    /*
        metoden omvandlar dicten till en formullerad lista av strings som
        beskriver de sedlar och mynt som användaren får tillbaka.
        i antal och namn samt singular eller plural.

        anävnder mig av en foreach för att lopa igen entrys och efter keyn matchar jag om
        det sedel sen om det inte är öre för att täcka alla valörer.

        om det är en sedel så adderas en string till listan och formateras med hjälp av
        IfBill metoden som tar in ett Kvp.

        om det inte är öre så är det ett annat mynt och då görs samma sak men
        med motoden IfCoi istället som gör samma sak fast för mynt.

        annars så skrivs värdet av en 50 öring.
    */
    protected override void FormatStrings(Dictionary<string, decimal> currencies)
    {
        foreach (var val in currencies)
        {
            if (
                val.Key == "Tusen"
                || val.Key == "Femhundra"
                || val.Key == "Tvåhundra"
                || val.Key == "Hundra"
                || val.Key == "Femtio"
                || val.Key == "Tjugo"
            )
                physicalCurrency.Add(IfBill(val));
            else
                physicalCurrency.Add(IfCoin(val));
        }
    }

    /*
        tar in ett kvp och om formaterar det till en string med hjälp av
        string interpolation och ternary operator för att formatera singular eller plural.
    */
    protected override string IfBill(KeyValuePair<string, decimal> entry) =>
        $"{entry.Value} {entry.Key} {(entry.Value > 1 ? "Lappar" : "Lapp")}.";

    protected override string IfCoin(KeyValuePair<string, decimal> entry) =>
        $"{entry.Value} {entry.Key} {(entry.Value > 1 ? "Kronor" : "Krona")}.";
}
