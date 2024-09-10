namespace BankSimDeepDive;

// Klass som ansvarar för att skapa och hålla koll på masterpassword samt kryptering av filer.
public class EncryptionHandler(CeasarCypher cypher, TextRepo repo)
{
    private readonly CeasarCypher _cypher = cypher;
    private readonly TextRepo _repo = repo;

    // Enviroment.NewLine för att programmet ska funka på mer än bara windows.
    private readonly string _newLine = Environment.NewLine;
    private int _masterPassword;

    /*
        Metod för att kolla om det finns ett masterpassword redan.

        om det inte finns körs CreateMasterPassword och Metoden returerar true.

        om det finns så används GetMasterPassword för att få en int.
        Med denna int så generas listan för _cyphers metoder.
        Sen använder jag ReadFromFile som ger tbx en lista som jag Indexar 0 för att få ut
        en string som är den krypterade versionen av masterpassword som redan angets vi tidigare
        session.

        med hjälp av int.TryParse och Decode som tar det angivna lösenordet och stringen vi hämta
        för att avkryptera och om dom är lika så returerar vi true.

        annars används recursion för att Loopa tills programmet stängs av eller rätt lösenord anges.
    */
    public bool CheckMasterPassword(string masterPath)
    {
        Console.Clear();
        if (!File.Exists(masterPath))
        {
            CreateMasterPassword(masterPath);
            return true;
        }
        else
        {
            int master = GetMasterPassword();
            _cypher.PopulateList(master);
            string masterPass = _repo.ReadFromFile(masterPath)[0];
            if (
                int.TryParse(_cypher.Decode(master, masterPass), out int masterPassword)
                && masterPassword == master
            )
            {
                _masterPassword = masterPassword;
                return true;
            }
            else
            {
                Console.WriteLine("Invalid Password.");
                return CheckMasterPassword(masterPath);
            }
        }
    }

    /*
        Metod för att skapa ett nytt MasterPassword.
        tar en int med hjälp av tryParse där limiters är att siffran måste vara längre än 3 och mindre
        än max värdet som en int kan hålla annars skickas användaren tillbaka till början med
        recursion.

        inten används sen som param för populatelist som skapar krypterings listan som används för att
        kryptera masterpasswordet och bank filen i appen.

        med hjälp av _cyphers encode och inten vi fick samt ToString så får vi tillbaka
        den krypterade versionen av inten denna sparas internt i klassen som _masterPassword
        för senare bruk.

        och den krypterade version av masterpassword skrivs och sparas till en fil.
    */
    private void CreateMasterPassword(string masterPath)
    {
        Console.Write(
            $"Välj ett Master Password.{_newLine}Detta måste minst vara 4 siffror.{_newLine}-> "
        );
        if (
            int.TryParse(Console.ReadLine(), out int master)
            && master.ToString().Length > 3
            && master <= int.MaxValue
        )
        {
            _cypher.PopulateList(master);
            string masterPassword = _cypher.Encode(master, master.ToString());
            _repo.WriteToFile(masterPath, masterPassword);
            _masterPassword = master;
        }
        else
            CreateMasterPassword(masterPath);
    }

    /*
        Metoden returerar en int efter de kriterer som ett lösenord måste nå.
    */
    private int GetMasterPassword()
    {
        Console.Write("Enter the Your password.\n-> ");
        if (
            int.TryParse(Console.ReadLine(), out int master)
            && master.ToString().Length > 3
            && master < int.MaxValue
        )
            return master;

        Console.WriteLine("Invalid Password.");
        return GetMasterPassword();
    }

    /*
        Skickar en string till Encode och masterpassword får tillbaka den krypterade versionen
        av stringen.
    */
    public string EncodeString(string movement) => _cypher.Encode(_masterPassword, movement);

    /*
        Metod för att avkryptera en fil rad för rad och returera en lista av strings.
        metoden skapar 2 listor en med texten från filen var pathen som anges leder till.
        och en tom lista där de avkrypterade raderna läggs till innan de retureras.

        Använder mig av en Foreach loop för att lätt kunna ta varje string i listan till
        Decode metoden i _cypher.
    */
    public List<string> DecodeStrings(string path)
    {
        List<string> listOfMovements = _repo.ReadFromFile(path);
        List<string> decodedMovements = [];
        foreach (string movement in listOfMovements)
            decodedMovements.Add(_cypher.Decode(_masterPassword, movement));

        return decodedMovements;
    }
}
