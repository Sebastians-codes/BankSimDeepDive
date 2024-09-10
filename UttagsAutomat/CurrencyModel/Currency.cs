namespace CashRegister;

// Vad en valuta är ett namn och värdet på valutan.

/*
    använder en primary constructor här för att jag tycker det ser bra ut
    på en så liten klass.

    get init för att du ska kunna hämta värdena men bara sätta dom vid
    skapandet av objektet.
*/
public class Currency(string name, decimal value)
{
    public string Name { get; init; } = name;
    public decimal Value { get; init; } = value;
}
