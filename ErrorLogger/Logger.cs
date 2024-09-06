using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister
{
    // Crash Logg tar all använderbar information och sparat till en txt fil.
    public class Logger(string logFileName)
    {
        private readonly string _logFileName = logFileName;

        public void LogError(Exception ex)
        {
            // @ för multi line string och interpolation
            // Använder mig av DateTime för att få exakt datum på när det hände.
            string entry =
                $@"[{DateTime.Now}]
            Exception message: {ex.Message}
            Stack trace: {ex.StackTrace}
            
            ";

            File.AppendAllText(_logFileName, entry);
        }
    }
}
