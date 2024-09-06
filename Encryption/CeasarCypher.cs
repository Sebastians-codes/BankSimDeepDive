using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

namespace BankSimDeepDive
{
    /*
        Klass som ansvarar för hur krypteringen fungerar.
        Har försökt hålla mig till char för att göra denn klassen så effektiv som möjligt.
    */
    public class CeasarCypher
    {
        // Arrayen som håller alla tecken som används för krypteringen.
        private char[] _chars;

        /*
            metod för att kryptera en string med _chars arrayen.
            Först initialiseras en tom array av chars med längeden av string som givits.
            Sen i foreach loopen så gör jag shift vilket är masterPasswordet och gör
            det till en string och med foreach loopen får jag ut varje inviduell siffra.

            Denna char parsar jag till en int men hjälp av int.parse och ToString eftersom
            du inte kan parsa en char till int.

            I For loopen som körs lika många gången som Ordet är långt så.
            Assignar en char till ordets bokstav med hjälp av i som index.

            om _chars arrayen inte innhåller bokstaven så returerar jag bokstaven
            oförändrad.

            annars så tar jag indexen av vart bokstaven i ordet är i _chars arrayen.
            Och använder sedan den för att returera den nya bokstaven.
            Returerar bokstaven som är på indexet som togs ovan + siffran från foreach loopen
            och med hjälp av modulus operator så ser vi till att vi inte går utan för listan
            
            låt säga listan är 26 lång bokstaven vi har är på index 24
            och 24 + siffran från foreach loopen blir mer än 26 vi säger att det blev 30.
            då får vi en rest på 4 som då kommer bli den nya indexen för bokstaven som vi
            returerar till chars arrayen.

            när foreach loopen har gått igenom alla siffror i shift så retureras
            en string med den krypterade texten.

            Detta med hjälp av string.Join() där första paramen är seperator där ""
            representerar ingen seperator alls. den andra paramen är IEnumerablen som
            du vill göra till en string.
        */
        public string Encode(int shift, string letter)
        {
            var chars = new char[letter.Length];
            foreach (char ch in shift.ToString())
            {
                int numShift = int.Parse(ch.ToString());
                for (int i = 0; i < letter.Length; i++)
                {
                    char currentChar = letter[i];
                    if (!_chars.Contains(currentChar))
                    {
                        chars[i] = currentChar;
                    }
                    else
                    {
                        int index = Array.IndexOf(_chars, currentChar);
                        chars[i] = _chars[(index + numShift) % _chars.Length];
                    }
                }
            }
            return string.Join("", chars);
        }

        /*
            Decode metoden är nästan exakt samma som Encode.
            Jag vet att jag kunde gjort en metod för båda men jag tycker
            att detta är trevligare även fast det bryter mot DRY
            och att som hade behövts är en till param och en if.

            Det som skiljer Decode från Encode är uträkningen av det nya indexet.

            detta för att vi nu tar minus shiften så om indexen var 1 - shift vi säger 5
            så får vi -4 men det funkar ju inte för då går vi out of bounds på arrayen.
            så då tar vi -4 + längden av _chars arrayen innan vi använder modulus med längden av _chars också.

            anledning till att vi plussar på längden av listan är för att om index nu blir - 4 + längd så kommer siffran
            vi få vara inom bounds samt rätt om man tänker sig arrayen vara en cirkel.
            och om index hade varit positiv redan så hade längden vi lagt till ändå försvunnit med modulus efter som de
            har samma värde.
        */
        public string Decode(int shift, string letter)
        {
            var chars = new char[letter.Length];
            foreach (char ch in shift.ToString())
            {
                int numShift = int.Parse(ch.ToString());
                for (int i = 0; i < letter.Length; i++)
                {
                    char currentChar = letter[i];
                    if (!_chars.Contains(currentChar))
                    {
                        chars[i] = currentChar;
                    }
                    else
                    {
                        int index = Array.IndexOf(_chars, currentChar);
                        chars[i] = _chars[((index - numShift) + _chars.Length) % _chars.Length];
                    }
                }
            }
            return string.Join("", chars);
        }

        /*
            metoden som ser till att all kryptering fungerar som den ska
            Denna kallas från EncryptionHandler vid start av programmet för att se till att
            programmet får tillgång till rätt lista att kryptera och avkryptera mot.
        */
        public void PopulateList(int masterPass)
        {
            int shuffleAmount = GetShuffleAmount(masterPass);
            MixAndAddToList();
            ShuffleAndBuffleList(shuffleAmount);
        }

        /*
            Generar ett värde som används för att skapa en unik lista genom
            att göra masterPasswordet till en string sen ta den första och den fjärde siffran
            göra dom till en int och returna det gånger 3.
        */
        private int GetShuffleAmount(int masterPass)
        {
            string master = masterPass.ToString();
            return int.Parse($"{master[0]}{master[3]}") * 3;
        }

        /*
            initaliserar _char arrayen med den sammalangda längen av lower, upper och number.
            Sen tar metoden en char från varje array i ordning tills _char arrayen är fylld.
            counter används för att hålla koll på vilken index som är näst i _chars arrayen.
        */
        private void MixAndAddToList()
        {
            char[] lower = AddUpperCaseChars();
            char[] upper = AddLowerCaseChars();
            char[] number = AddNumbersAndSpecial();
            _chars = new char[lower.Length + upper.Length + number.Length];
            int counter = 0;

            for (int i = 0; i < _chars.Length; i++)
            {
                if (i < lower.Length)
                {
                    _chars[counter] = lower[i];
                    ++counter;
                }

                if (i < upper.Length)
                {
                    _chars[counter] = upper[i];
                    ++counter;
                }

                if (i < number.Length)
                {
                    _chars[counter] = number[i];
                    ++counter;
                }
            }
        }

        /*
            metod för att göra listan så slumpad som möjligt men ändå alltid lika varje gång.
            metoden använder sig av två for loops den första är för att skapa en unik lista
            för varje masterpassword.

            den andra loopen tar en sektion ur _char arrayen och lägger till den i temps listan
            på det indexet som j är vilket ökar med 1 varje itteration.

            men även sektionen som vi tar ändras varje itteration eftersom GetRange låter dig
            ange både start index och hur många items man vill ta efter den.
            Där tar jag start index som j och slut index som längden av _char arrayen / j + 1
            Detta för att sektionen vi tar varje itteration ska vara annorlunda från den förra.

            Sen varje gång i är ett jämt tal så Omvänder jag Sektionen vi tog innan den läggs till
            i temps listan.

            Detta skapar en unik lista beroende på shuffle värdet som generas från masterpasswordet.

            till slut för att se till att _chars arrayen har rätt värden i sig så använder jag mig av
            ToHashSet eftersom Det bara kan hålla 1 av varje unikt värde så alla dubletter försvinner
            Sen gör jag den till en array av chars igen.
        */
        private void ShuffleAndBuffleList(int shuffle = 10)
        {
            for (int i = 0; i < shuffle; i++)
            {
                List<char> temps = [];
                int lengthOfList = _chars.Length;
                for (int j = 0; j < lengthOfList; j++)
                {
                    List<char> temp = _chars.ToList<char>().GetRange(j, lengthOfList / (j + 1));
                    if (i % 2 == 0)
                        temp.Reverse();
                    temps.InsertRange(j, temp);
                }
                _chars = temps.ToHashSet<char>().ToArray<char>();
            }
        }

        // Skapar en Char Array Med alla Stora bokstäver
        private char[] AddUpperCaseChars()
        {
            int min = 65,
                max = 90;
            var chars = new char[max - min + 1];
            for (int i = min; i <= max; i++)
                chars[i - min] = ((char)i);
            return chars;
        }

        // Skapar en Char Array Med 1 till 9 samt en del Special tecken
        private char[] AddNumbersAndSpecial()
        {
            int min = 32,
                max = 65;
            var chars = new char[max - min + 1];
            for (int i = min; i < max; i++)
                chars[i - min] = ((char)i);
            return chars;
        }

        // Skapar en Char Array Med alla Små bokstäver
        private char[] AddLowerCaseChars()
        {
            int min = 97,
                max = 122;
            var chars = new char[max - min + 1];
            for (int i = min; i <= max; i++)
                chars[i - min] = ((char)i);
            return chars;
        }
    }
}
