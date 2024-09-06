using BankSimDeepDive;
using CashRegister;

// initaliserar de instancer av objekt som behös för att programmet ska fungera.
Logger logger = new("CrashLog.txt");
TextRepo repo = new();
App app =
    new(
        new UttagsAutomat(new SwedishCurrencyModel(), new SwedishStandardMessages()),
        new UserInput(),
        repo,
        new EncryptionHandler(new CeasarCypher(), repo)
    );

// Global try catch för att fånga buggar som jag inte har hittat ännu.
try {
    app.Run();
}
catch (Exception ex) {
    Console.WriteLine("Unexpected error, The program will close.");
    logger.LogError(ex);
    Console.ReadKey();
}
