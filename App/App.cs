using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashRegister;

namespace BankSimDeepDive
{
    // Klass som hanterar hela Programmets flöde.

    /*
        Använder mig av Primary Constructor för att jag tycker det är mer
        lättläsligt.

        använder Dependency Injection för att få tillgång till metoder från
        andra klasser.
    */
    public class App(
        // Använder Mig av förra delens deepdive jag gjorde för att simulera en uttags automat.
        UttagsAutomat uttagsAutomat,
        UserInput userInput,
        TextRepo repo,
        EncryptionHandler encryption
    )
    {
        private readonly UttagsAutomat _uttagsAutomat = uttagsAutomat;
        private readonly UserInput _userInput = userInput;
        private readonly TextRepo _repo = repo;
        private readonly EncryptionHandler _encryption = encryption;

        // Enivoment.NewLine för att programmet ska funka på mer än windows.
        private readonly string _newLine = Environment.NewLine;

        // strings för att sätta fil paths för de filer som programmet behöver.
        private string _path,
            _pathMaster;

        // Lista av strings som är strings av alla insättningar och uttag i programmet.
        private List<string> _movements;

        // för att spara användar input. Och Skriva en string till fil.
        private string _userChoice,
            _transaction;

        // decimal för att representera valuta så bra som möjligt.
        private decimal _movement,
            _intrest,
            _totalIntrest,
            _totalSaved,
            _balance;

        // float för att representera en ränta på bästa sätt.
        private float _rate;

        // int för att år behöver inte representeras med högre tal i detta program.
        private int _year;

        /*
            Mainloopen av programmet.
            först kör vi metoden för at se om ett masterpassword måste skapas eller anges.
            denna kommer alltid returera true eftersom den loopar internt till önskat resultat
            har uppnåtts.

            Sedan laddar vi tidigare Saldo flytta om det finns några.

            efter det skrivs menyn ut och vi tar input från användaren.
            
            metoden som används för do while loopen kör den metod som användaren vill köra
            och returerar true så do do while loopen börjar om.
            Men om a angest för avsluta så retureras false och då avslutas loopen.
        */
        public void Run()
        {
            if (!_encryption.CheckMasterPassword(_pathMaster))
                return;
            GetSavedBalance();
            do
            {
                _userChoice = _userInput.MenuChoice();
            } while (SwitchFlow());
        }

        /*
            En del av uppgiften var att använda oss av en switch men jag ville använda en
            switch expression för att köra min metoder.
            ända problemmet var att man måste returera ett värde från varje case
            detta är inte fallet i en vanlig switch.

            men jag löste det genom att ta och göra alla metoder till att returera bools
            och sedan använda den för att hålla programmet i liv.

            om i körs DepositAndWithdrawl med true för att insättnings metoden ska köras.
            om u körs samma men med false för uttags metoden ska köras.
            om s körs metoden för at visa aktuellt saldo samt alla transactioner.
            om r körs metoden för att räkna ut avkastning.
            om a så avslutas programmet.
            om _ detta kan inte hända.
        */
        private bool SwitchFlow()
        {
            return _userChoice switch
            {
                "i" => DepositAndWithdrawal(true),
                "u" => DepositAndWithdrawal(false),
                "s" => ShowBalance(),
                "r" => Intrest(),
                "a" => GoodByeMessage(),
                _ => false
            };
        }

        private void GetUser()
        {
            while (true)
            {
                Console.Write("Enter your account number 4 numbers long -> ");
                if (int.TryParse(Console.ReadLine(), out int num) && num.ToString().Length == 4)
                {
                    _path = $"{num}Bank.txt";
                    _pathMaster = $"{num}MPass.txt";
                    break;
                }
                Console.Clear();
                Console.WriteLine("Invalid account number try again.");
            }
        }

        /*
            Metod för att hämta data från den krypterade spara filen med alla transactioner
            för att räkna ut saldo.

            med hjälp av encryption klassen så avkrypterar vi filen och får tillbaka en Lista av strings
            Med en foreach loop så lägger vi till beloppen till saldot med hjälp av
            decimal.Parse eftersom vi vet att det alltid kommer vara en decimal.
            jag splittar stringen vid första k och index 0 som kommer representera 100,00kr så vi får 100,00
        */
        private void GetSavedBalance()
        {
            _movements = _encryption.DecodeStrings(_path);
            foreach (var movement in _movements)
                _balance += decimal.Parse(movement.Split("k")[0]);
        }

        /*
            Metod för att komtrollera insättning och uttag.
            Om parametern är true
            körs Insättnings metoden
            om false gör uttags metoden.

            annars är kontot tomt och det finns inga pengar att ta ut.

            returerar true för att programmet ska fortsätta.
        */
        private bool DepositAndWithdrawal(bool deposit)
        {
            if (deposit)
                Deposit(deposit);
            else if (_balance > 0)
                Withdrawl(deposit);
            else
                Console.WriteLine("Du har inga pengar at ta ut.");

            return true;
        }

        /*
            Metod för att visa aktuellt saldo samt de transactioner som har hänt.

            metoden använder sig av encyption DecodeStrings som returerar den avkrypterade filen
            av transactioner.
            Använder Reverse för att få den senaste transactionen först i listan.

            Sedan med hjälp av en foreach loop skrivs varje transaction ut.

            metoden returerar true för att programmet ska fortsätta köra.
        */
        private bool ShowBalance()
        {
            Console.Clear();
            Console.WriteLine($"Your account balance : {_balance:N2}kr.{_newLine}");
            _movements = _encryption.DecodeStrings(_path);
            _movements.Reverse();
            foreach (string movement in _movements)
                Console.WriteLine(movement);
            returnToMenu();
            return true;
        }

        /*
            Metod för att sätta in pengar på konto.
            Använder Metoden BankMovemnt som tar en bool för om det är en insättning eller uttag.
            denna veriferar att summan som användaren anger och reuterar en decimal.

            uppdaterar saldot med summan.
            formaterar en string som sedan krypteras och sparas till bank filen.
        */
        private void Deposit(bool deposit)
        {
            _movement = _userInput.BankMovement(deposit, _balance);
            _balance += _movement;
            _transaction = $"{_movement:N2}kr. Balance:{_balance:N2}kr.{_newLine}";
            _transaction = _encryption.EncodeString(_transaction);
            _repo.WriteToFile(_path, _transaction);
        }

        /*
            Metod för uttag.
            Med hjälp från BankMovment så veriferar den värdet och returerar det.
            om värdet är har en decimal som är över 0.5 så ändras uttags beloppet till
            närmsta krona avrundat uppåt annars avrundas det till närmsta krona neråt.

            om användaren försöker ta ut 0 så kommer man tillbaka till menyn utan några bieffekter.

            summan tas bort från saldot.
            en string formatteras som sedan krypteras och sparas till bank filen.
            Sedan simuleras en uttags automat som ger dig dina pengar i fysik valuta.

            Sedan åter till menyn.
        */
        private void Withdrawl(bool deposit)
        {
            _movement = _userInput.BankMovement(deposit, _balance);

            if (_movement % 1 >= 0.50m)
                _movement = (int)_movement + 1;
            else if (_movement == 0)
            {
                returnToMenu();
                return;
            }
            else
                _movement = (int)_movement;

            _balance -= _movement;
            _transaction = $"-{_movement:N2}kr. Balance:{_balance:N2}kr.{_newLine}";
            _transaction = _encryption.EncodeString(_transaction);
            _repo.WriteToFile(_path, _transaction);
            _uttagsAutomat.RunBankVersion(_movement);
            returnToMenu();
        }

        /*
            Metoden för att ränka ut avkastning från en räntestas ett årligt spar belopp samt antal år.
            Med hjälp av IntrestValues som tar input och validerar sedan returerar en Value tuple
            med de tre värde vi behöver.

            använder en for loop som loopar för antal år
            varje itteration så sparas sparvärdet till totalt sparat.
            sedan räknas avkastningen ut genom att ta totala sparbeloppet * räntesatsen / 100
            detta värde läggs till på både totalAvkastning och totala sparbeloppet.

            Sedan formatteras en string som skriver året hur mycket man har sparat och hur mycket
            avkastningen va på.

            när loopen är klar så kommer en mer detaljerad redovisning som skriver hur mycket du sparat
            årligen i hur många år med vilken ränta.

            Sedan skrivs det total sparbeloppet ut samt den totala avkastningen.

            den totalaAvkastningen och det totala sparbeloppet nollställs ifall användaren vill räkna igen.

            metoden returerar true sedan för att Main loopen ska fortsätta.
        */
        private bool Intrest()
        {
            Console.Clear();
            (_rate, _movement, _year) = _userInput.IntrestValues();
            for (int i = 0; i < _year; i++)
            {
                _totalSaved += _movement;
                _intrest = _totalSaved * ((decimal)_rate / 100);
                _totalIntrest += _intrest;
                _totalSaved += _intrest;
                Console.WriteLine(
                    $"År:{i + 1} Total Sparat belopp:{_totalSaved:N2}kr. - Avkastning:{_intrest:N2}kr."
                );
            }
            Console.WriteLine(
                $"{_newLine}Totalt efter att ha sparat {_movement:N2}kr i {_year}år med ränta på {_rate}%.{_newLine}"
                    + $"Total Summa:{_totalSaved:N2}kr. Totalt tjänat på ränta:{_totalIntrest:N2}kr."
            );
            _totalIntrest = 0;
            _totalSaved = 0;
            returnToMenu();
            return true;
        }

        /*
            Metod för att säga tack och ge ett avsluts meddelande till användaren.
            
            returerar false för att avsluta mainloopen så att programmet avslutas.
        */
        private bool GoodByeMessage()
        {
            Console.WriteLine("Tackar tackar ha det gött.");
            Console.ReadKey();
            return false;
        }

        // Metod för att göra programmet mer lättförståeligt och användar vänligt.
        private void returnToMenu()
        {
            Console.WriteLine($"{_newLine}Tryck på en tangent för att återgå till menyn.");
            Console.ReadKey();
        }
    }
}
