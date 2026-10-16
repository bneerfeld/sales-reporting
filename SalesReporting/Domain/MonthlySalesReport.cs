namespace SalesReporting.Domain;

public sealed record MonthlySalesReport(int Year, int Month, decimal TotalUsd);