using Ardalis.SmartEnum;

namespace SalesReporting.Domain;

public sealed class Currency : SmartEnum<Currency>
{
    public static readonly Currency Usd = new("USD", 1, 1.0m);
    public static readonly Currency Eur = new("EUR", 2, 1.1m);
    public static readonly Currency Gbp = new("GBP", 3, 1.3m);

    public decimal UsdRate { get; }

    private Currency(string name, int value, decimal usdRate) : base(name, value) 
        => UsdRate = usdRate;

    public decimal ToUsd(decimal amount) => amount * UsdRate;
}