using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister
{
    /*
        Abstarct baseclass för att göra implementationen av flera valörer
        main classen håller listan av Currency classen som håller värdet av valören.
        samt listan som sen printas ut till användaren efter konvertering till fysisk valuta.

        använder mig av protected för att dessa metoder ska bara vara synliga för de klasser
        som inheritar från denna baseclass

        de som är public är de metoder som används av instancer av ett
        objekt som inheritar från CurrencyModel.
    */
    public abstract class CurrencyModel
    {
        protected List<Currency> currencies;
        protected List<string> physicalCurrency;

        // suffix är vad valutan sluta på t.ex kr sek $ €
        protected string _suffix;

        public CurrencyModel()
        {
            currencies = [];
            physicalCurrency = [];
        }

        // metod för addera ett objekt av Currency till listan av valörer.
        protected void Add(string name, decimal value) => currencies.Add(new Currency(name, value));

        /*
            Den ända metoden som är den samma över alla olika valörer
            denna skriver ut antar sedlar och mynt på ett roligt sätt med hjälp av en
            foreach loop och en for loop inuti som skriver - 10 gånger med 150ms mellanrum
            mellan varje valör.

            efter hela listan har skrivits ut så töms den inför en ny uträkning.
        */
        public virtual void PrintCurrency()
        {
            Console.WriteLine("BrrRrrRRRrr..");
            foreach (string currency in physicalCurrency)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write("-");
                    System.Threading.Thread.Sleep(150);
                }
                Console.WriteLine($"\n{currency}");
            }
            physicalCurrency.Clear();
        }

        // metod för att hämta suffixen för valutan.
        public virtual string GetSuffix()
        {
            return _suffix;
        }

        // abstracta metoder som måste implementeras av en valuta för att den ska fungera.
        public abstract void ConvertToPhysical(decimal currency);

        protected abstract void FormatStrings(Dictionary<string, decimal> currencies);

        protected abstract string IfBill(KeyValuePair<string, decimal> entry);

        protected abstract string IfCoin(KeyValuePair<string, decimal> entry);
    }
}
