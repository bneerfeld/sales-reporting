using SalesReporting.Domain;

namespace SalesReporting.Tests.Domain;

public sealed class CurrencyTests
{
    public static TheoryData<Currency, decimal, decimal> WholeAmounts => new()
    {
        { Currency.Usd, 100m, 100m },
        { Currency.Eur, 100m, 110m },
        { Currency.Gbp, 100m, 130m }
    };

    public static TheoryData<Currency, decimal, decimal> FractionalAmounts => new()
    {
        { Currency.Usd, 0.1m, 0.1m },
        { Currency.Eur, 0.1m, 0.11m },
        { Currency.Gbp, 0.1m, 0.13m },
        { Currency.Eur, 1234.56m, 1358.016m },
        { Currency.Gbp, 0.01m, 0.013m }
    };

    public static TheoryData<Currency> AllCurrencies => new(Currency.List);

    [Theory]
    [MemberData(nameof(WholeAmounts))]
    public void ToUsd_ConvertsAmountWithRate(Currency currency, decimal amount, decimal expectedUsd)
    {
        decimal actualUsd = currency.ToUsd(amount);

        Assert.Equal(expectedUsd, actualUsd);
    }

    [Theory]
    [MemberData(nameof(FractionalAmounts))]
    public void ToUsd_KeepsDecimalPrecision(Currency currency, decimal amount, decimal expectedUsd)
    {
        decimal actualUsd = currency.ToUsd(amount);

        Assert.Equal(expectedUsd, actualUsd);
    }

    [Theory]
    [MemberData(nameof(AllCurrencies))]
    public void ToUsd_ZeroAmount_ReturnsZero(Currency currency)
    {
        decimal actualUsd = currency.ToUsd(0m);

        Assert.Equal(0m, actualUsd);
    }
}
