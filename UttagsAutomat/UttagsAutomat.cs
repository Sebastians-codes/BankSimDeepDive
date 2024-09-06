using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister
{
    /*
        Register klassen är appen som körs när användern har valt språk och currency model.
        Är fortfarande väldigt ny till programmering men har försökt tänka på
        SRP, Data hiding, Encapsulation samt Dependency Injection.
    */
    public class UttagsAutomat
    {
        // Deklarerar de variabler jag använder mig av i klassen.
        private readonly IStandardMessages _messages;
        private readonly CurrencyModel _currencyModel;
        private readonly List<decimal> _shoppingCart;

        // decimal för att det ger höst precision när man jobbar med valuta och det är att föredra.
        private decimal _priceOfItems;
        private decimal _pricePayedForItems;
        private decimal _change;
        private bool on;
        private char choice;
        private string _suffix;

        // Constructor som initsierar _shoppCart Listan samt _messages, _currencyModel och _suffix.
        public UttagsAutomat(CurrencyModel currencyModel, IStandardMessages messages)
        {
            _messages = messages;
            _currencyModel = currencyModel;
            _shoppingCart = [];
            _suffix = _currencyModel.GetSuffix();
        }

        /*
            Flow Control


            1 -> Först om payed eller om priceOfItems || _shoppingcart inte är tomma
                 printas ett medellande om hur mycket som har spenderats och betalats
                 för att lättare se om man har betalat tillräckligt.

            2 -> Visar menyn och de alternativ använder har att välja mellan
                 1 lägg till en produkt.
                 2 lägg till hur mycket du har betalt.
                 3 räkna ut hur mycket du får i växel.
            
            3 -> Tar user input.
                 Om det är ett korrekt val så startas Flow Controllen för
                 vilken metod som ska köras.
            
            4 -> Avslutar applicationen

            om något annat anges så får användaren ett error meddelande och skickas tillbaka
            till början av metoden.
        */

        public void RunBankVersion(decimal amount)
        {
            _change = amount;
            GiveMoney();
        }

        private void Run()
        {
            Console.Clear();
            _messages.WelcomeMessage();

            on = true;

            while (on)
            {
                if (_pricePayedForItems > 0 || _priceOfItems > 0 || _shoppingCart.Count > 0)
                {
                    Console.Clear();
                    _messages.ShowCurrentPayedNSpent(
                        _shoppingCart.Sum(),
                        _pricePayedForItems,
                        _suffix
                    );
                }
                _messages.ShowMenu();
                if (char.TryParse(Console.ReadLine(), out choice))
                {
                    Console.Clear();
                    GetUserChoice();
                }
                else
                {
                    Console.Clear();
                    _messages.ErrorTryAgain();
                }
            }
        }

        /*
            Flow Control som görs efter vi har fått ett korrekt val
            från användaren.
        */
        private void GetUserChoice()
        {
            switch (choice)
            {
                case '1':
                    // adderar ett värde till listan.
                    _messages.HowMuchSpent();
                    _shoppingCart.Add(GetValue());
                    break;
                case '2':
                    // adderar värdet till decimal variablen payed.
                    _messages.HowMuchPayed();
                    _pricePayedForItems += GetValue();
                    break;
                case '3':
                    // kör logiken för att visa växel och konvertering till fysisk valuta.
                    ProcessChange();
                    break;
                case '4':
                    // avslutar applicationen.
                    on = false;
                    break;
                default:
                    // Stardard meddelande för felaktigt val.
                    _messages.ErrorTryAgain();
                    break;
            }
        }

        /*
            Metoden som visar hur mycket som kunden kommer få
            tillbaka i den valutan som är vald och antal sedlar och mynt.
        */
        private void ProcessChange()
        {
            // Kollar så att användaren har lagt in något att betala för.
            if (!CheckIsItemAdded())
            {
                // Standard meddelande för om det inte finns några produkten i korgen.
                _messages.NoItemsInCart();
                return;
            }
            // Kollar så att användaren har betalat tillräckligt för att betala för produkten.
            if (!CheckPayedEnough())
            {
                // standard meddelande för om användaren inte har betalat tillräckligt.
                _messages.NotPayedEnough();
                return;
            }
            // Adderar totalen av alla produkter i _shoppingCart listan till decimal variablen _priceOfItems.
            CalculateSumOfWares();
            _change = _pricePayedForItems - _priceOfItems;
            if (_change > 0m)
            {
                // Om _change är mer 0 så körs konverterings metoden till fysisk valuta.
                GiveMoney();
            }
            else
            {
                // Standard meddelande om användaren har betalat like mycket som varand kostade.
                _messages.PayedEven();
            }
            // Standard meddeland. frågar om man vill starta om applicationen.
            _messages.RunAgain();
            if (Console.ReadLine().ToLower() != "y")
            {
                on = false;
            }
            Console.Clear();
            // Sätter alla variabler som räknar ut växel till 0 för att kunna göra en ny uträkning.
            ResetState();
        }

        // metod för att resetta för en omstart av applicationen.
        private void ResetState()
        {
            _priceOfItems = 0;
            _pricePayedForItems = 0;
            _change = 0;
        }

        // veriferar om det finns något i listan av produkter.
        private bool CheckIsItemAdded()
        {
            if (_shoppingCart.Count > 0)
            {
                return true;
            }
            return false;
        }

        // veriferar att summan som är betald är lika med eller mer än summan i korgen.
        private bool CheckPayedEnough()
        {
            // Använder mig av den nativa metoded Sum på List för att räkna ut totalen av alla produkter i listan.
            if (_pricePayedForItems >= _shoppingCart.Sum())
            {
                return true;
            }
            return false;
        }

        /*
            Adderar totalen av alla produkter i korg listan till en variabel inför uträkning av växel.
            Tömmer Korg listan.
        */
        private void CalculateSumOfWares()
        {
            foreach (var item in _shoppingCart)
            {
                _priceOfItems += item;
            }
            _shoppingCart.Clear();
        }

        // Veriferar att användaren ger ett korrekt nummer och att det är mer än 0.
        // använder recursion istället för en loop.
        private decimal GetValue()
        {
            if (decimal.TryParse(Console.ReadLine(), out var number) && number > 0)
                return number;
            else
            {
                _messages.ErrorTryAgain();
                return GetValue();
            }
        }

        // metoden som visar hur mycket pengar användaren får tillbaka.
        /*
            Modifierat metoden till public eftersom jag vill använda denna metod
            i min bank app.
        */
        private void GiveMoney()
        {
            Console.Clear();
            //_messages.ShowChange(_change, _suffix);
            Console.WriteLine($"Du har tagit ut {_change:N2}kr.");
            _messages.ConverterWorking();
            _currencyModel.ConvertToPhysical(_change);
            _currencyModel.PrintCurrency();
            Console.WriteLine("\nVar vänlig ta dina pengar.");
        }
    }
}
