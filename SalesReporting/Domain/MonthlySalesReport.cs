namespace SalesReporting.Domain;

public sealed record MonthlySalesReport(string ReportName, int Year, int Month, decimal TotalUsd);