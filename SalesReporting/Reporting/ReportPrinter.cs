using Microsoft.Extensions.Logging;
using SalesReporting.Domain;

namespace SalesReporting.Reporting;

public sealed class ReportPrinter(ILogger<ReportPrinter> logger)
{
    public void Print(MonthlySalesReport report)
    {
        logger.LogInformation("Monatlicher Verkaufsbericht {Month:D2}/{Year}", report.Month, report.Year);
        logger.LogInformation("Gesamtumsatz: {TotalUsd:N2} USD", report.TotalUsd);
    }
}