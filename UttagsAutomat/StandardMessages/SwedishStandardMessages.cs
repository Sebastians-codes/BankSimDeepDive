namespace CashRegister;

public class SwedishStandardMessages : IStandardMessages
{
    public void ConverterWorking() => Console.WriteLine("Tack för att du väntar.\n");

    public void ErrorTryAgain() => Console.WriteLine("Felaktig input. Försök igen.");

    public void HowMuchPayed() => Console.WriteLine("Hur mycket har du betalat?");

    public void HowMuchSpent() => Console.WriteLine("Hur mycket kostar produkten?");

    public void NoItemsInCart() => Console.WriteLine("Korgen är tom.");

    public void NotPayedEnough() => Console.WriteLine("Du har inte betalat tillräckligt.");

    public void PayedEven() => Console.WriteLine("Jämt upp!");

    public void RunAgain() => Console.Write("\nVill du börja en ny session? y/n?\n-> ");

    public void ShowChange(decimal change, string suffix) =>
        Console.WriteLine($"Du kommer få tillbaka {change}{suffix}.");

    public void ShowCurrentPayedNSpent(decimal spent, decimal payed, string suffix) =>
        Console.Write(
            $"Totalt värde i korgen: {spent}{suffix}.\nTotalt belopp betalat: {payed}{suffix}.\n"
        );

    public void ShowMenu()
    {
        Console.WriteLine("HuvudMeny");
        Console.WriteLine("1 -> Lägg till en produkt i korgen.");
        Console.WriteLine("2 -> Lägg till betalad summa.");
        Console.WriteLine("3 -> Få växel.");
        Console.Write("4 -> avsluta programmet.\n-> ");
    }

    public void WelcomeMessage() => Console.WriteLine("Välkommen!");
}
