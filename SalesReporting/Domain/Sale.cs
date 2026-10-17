namespace SalesReporting.Domain;

public sealed record Sale(DateOnly Date, decimal Amount, Currency Currency)
{
    public decimal AmountInUsd => Currency.ToUsd(Amount);
}