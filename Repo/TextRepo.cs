using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSimDeepDive
{
    // Klass som ansvara för att skriva till filer och hämta data från filer.
    public class TextRepo
    {
        /*
            tar en param som är pathen till filen som metoden ska läsa.
            om filen inte finns så retureras en tom lista.

            med hjälp av File.ReadAllLines som returerar en array som jag gör om till en lista
            av strings får vi alla rader som strings i en lista.
            Denna retureras från metoden
        */
        public List<string> ReadFromFile(string path)
        {
            if (!File.Exists(path))
                return [];
            return File.ReadAllLines(path).ToList<string>();
        }

        /*
            Denna metoden har två parametrar en för vart filen ska spara och en för vilken string
            som ska sparas.
            Med hjälp av File.AppendAllText som öppnar en fil och skriver den givna stringen till
            en ny rad i filen sedan stänger den.
        */
        public void WriteToFile(string path, string content) => File.AppendAllText(path, content);
    }
}
