namespace BankSimDeepDive;

/*
    Klass vars uppgift är att se till att alla metoder får rätt
    input från användaren och även ta input från användaren.
*/
public class UserInput
{
    // för att programmet ska fungera på mer än windows.
    private readonly string _newLine = Environment.NewLine;

    // decimal för att representera pengar på bästa sätt.
    private decimal _movement,
        _amount;

    // float för att ha floating point precision.
    private float _intrest;

    // string för att ta meny val från användaren
    private string _choice;

    // string array för att kunna kontrollera mot användaren input.
    private readonly string[] _choices = ["i", "u", "s", "r", "a"];

    // int för att du behöver inte större för att representera år i detta program.
    private int _year;

    /*
        Metod för att Printa Menyn och ta input från användaren samt validera
        inputen att den är korrekt och om det är så returerar inputen från användaren
        som en string.

        annars loopar vi med recursion.
    */
    public string MenuChoice()
    {
        PrintMenu();
        _choice = GetMenuInput();
        if (!ValidateUserChoice())
        {
            return MenuChoice();
        }
        return _choice;
    }

    /*
        Första gången jag använder en value tuple har använt liknande i python innan.
        Metod för att få de värdena som behövs för att räkna ut avkasnting och besparande
        efter angivna år.

        metoden returerar 3 värden i en tuple.
    */
    public (float intrest, decimal amount, int year) IntrestValues()
    {
        GetIntrestRate();
        GetSavingAmount();
        GetYear();

        return (_intrest, _amount, _year);
    }

    /*
        Metod för att få en räntesats från användaren
        använder float.TryParse och returerar det om det är >= än 0 och <= än 100
        eftersom % är 0 - 100.

        använder recursion tills vi får ett värde som klarar kriterierna.
    */
    private void GetIntrestRate()
    {
        Console.Write($"Hur mycket ränta vill du ränka med? räknas i %{_newLine}-> ");
        if (
            float.TryParse(Console.ReadLine(), out _intrest)
            && _intrest >= 0
            && _intrest <= 100
        )
            return;
        else
        {
            Console.Clear();
            Console.WriteLine("Ränta kan inte vara mindre än 0% eller mer än 100%");
            GetIntrestRate();
        }
    }

    /*
        Metod för att antal spar år från användaren
        Använder int.TryParse för att få ett år och kollar så det är mer än 0 men mindreeller lika med
        vad en int kan hålla.

        använder recursion för at loopa till användaren ger ett giltligt värde.
    */
    private void GetYear()
    {
        Console.Write(
            $"Hur många år vill du spara {_amount:N2}kr med {_intrest}% ränta?{_newLine}-> "
        );
        if (int.TryParse(Console.ReadLine(), out _year) && _year > 0 && _year <= int.MaxValue)
            return;
        else
        {
            Console.Clear();
            Console.WriteLine("Du måste spara i minst 1 år.");
            GetYear();
        }
    }

    /*
        Metod för att få Det Årliga spar beloppet från användaren.
        Med hjälp av decimal.TryParse tar jag input från användaren och kollar så
        den är mer än 0 men mindre eller lika med max värdet som decimal kan hålla.

        Använder recursion för att loopa till användaren ger ett giltligt värde.
    */
    private void GetSavingAmount()
    {
        Console.Write($"Hur mycket vill du spara årligen?{_newLine}-> ");
        if (
            decimal.TryParse(Console.ReadLine(), out _amount)
            && _amount > 0
            && _amount <= decimal.MaxValue
        )
            return;
        else
        {
            Console.Clear();
            Console.WriteLine("Du måste spara mer än 0kr.");
            GetSavingAmount();
        }
    }

    /*
        Metod för att verifera belopp att sätta in och ta ut.

        Metoden har två parametrar en bool för att kolla om det är en insättning eller ett
        uttag.
        en decimal för att kunna skriva saldot på konto vid utag samt kolla om det
        finns tillräckligt med pengar på konto för att kunna ta ut pengar.

        Metoden använder sig av en annan metod för att få summan som använder anger
        denna veriferar och returerar ett belopp.

        Om det är ett uttag så kontrollas det att det finns tillräckligt i saldo.
        Om inte skrivs ett meddelande att de inte kan ta ut mer än vad de har.

        Recursion används för att Loopa tills Användaren ger giltigt svar.
    */
    public decimal BankMovement(bool deposit, decimal balance)
    {
        Console.Clear();
        if (deposit)
        {
            Console.Write($"Hur Mycket vill du sätta in?{_newLine}-> ");
            return GetMovementInput();
        }
        Console.WriteLine($"Ditt saldo är {balance:N2}kr.");
        Console.Write($"Hur mycket vill du ta ut?{_newLine}-> ");
        GetMovementInput();
        if (_movement <= balance)
            return _movement;
        Console.WriteLine(
            $"Du kan inte ta ut mer pengar än va du har.{_newLine}"
                + $"Du har {balance:N2}kr på ditt konto."
        );
        return BankMovement(deposit, balance);
    }

    /*
        Metod för att verifera en summa.
        Med Hjälp av decimal.TryParse veriferar metoden att summan är valid och inte
        är mindre än 0 och större än vad en decimal kan hålla.

        Använder recursion tills användar anger ett giltligt värde.
    */
    private decimal GetMovementInput()
    {
        if (
            decimal.TryParse(Console.ReadLine(), out _movement)
            && _movement >= 0
            && _movement <= decimal.MaxValue
        )
            return _movement;
        Console.WriteLine("Felaktig Input, Ange ett nummer.");
        return GetMovementInput();
    }

    // Metod som input och gör det till lowercase om möjligt.
    private string GetMenuInput() => Console.ReadLine().ToLowerInvariant();

    //Metod för att verifera använda input är en av menyvalen.
    private bool ValidateUserChoice()
    {
        if (
            !string.IsNullOrEmpty(_choice)
            && !string.IsNullOrWhiteSpace(_choice)
            && _choices.Contains(_choice)
        )
        {
            return true;
        }
        return false;
    }

    // Metod som skriver ut Menyn.
    private void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("Fort Notx");
        Console.Write(
            "[I]nsättning"
                + $"{_newLine}[U]ttags Automat"
                + $"{_newLine}[S]aldo"
                + $"{_newLine}[R]änta Spar uträkning"
                + $"{_newLine}[A]vsluta program."
                + $"{_newLine}-> "
        );
    }
}
