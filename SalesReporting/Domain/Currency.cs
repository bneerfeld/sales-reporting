using Ardalis.SmartEnum;

namespace SalesReporting.Domain;

public sealed class Currency : SmartEnum<Currency>
{
    public static readonly Currency Usd = new("USD", 0, 1.0m);
    public static readonly Currency Eur = new("EUR", 1, 1.1m);
    public static readonly Currency Gbp = new("GBP", 2, 1.3m);

    private readonly decimal _usdRate;

    private Currency(string name, int value, decimal usdRate) : base(name, value) => _usdRate = usdRate;

    public decimal ToUsd(decimal amount) => amount * _usdRate;
}