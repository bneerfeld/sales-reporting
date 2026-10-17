using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Tests.Fakes;

public sealed class InMemoryReportWriter : IReportWriter
{
    private readonly List<MonthlySalesReport> _writtenReports = [];

    public IReadOnlyList<MonthlySalesReport> WrittenReports => _writtenReports;

    public Task<string> WriteAsync(MonthlySalesReport report, CancellationToken cancellationToken)
    {
        _writtenReports.Add(report);

        return Task.FromResult($"{report.ReportName}.json");
    }
}
