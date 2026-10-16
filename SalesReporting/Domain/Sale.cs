namespace SalesReporting.Domain;

public sealed record Sale(DateTime Date, decimal Amount, Currency Currency)
{
    public decimal AmountInUsd => Currency.ToUsd(Amount);
}