using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SalesReporting.Domain;
using SalesReporting.Reporting;
using SalesReporting.Tests.Fakes;

namespace SalesReporting.Tests.Reporting;

public sealed class ReportJobTests
{
    private const int Year = 2026;
    private const int Month = 10;

    private static readonly IReadOnlyList<Sale> OctoberSales = [new Sale(new DateTime(2026, 10, 1), 100m, Currency.Usd)];

    private readonly InMemoryReportWriter _writer = new();

    [Fact]
    public async Task RunAsync_WithoutReport_WritesAllReportsAndSucceeds()
    {
        InMemorySaleRepository repository = new(SalesFor("frankfurt", "berlin"));
        string[] expected = ["berlin", "frankfurt"];

        ExitCode exitCode = await RunAsync(repository, report: null);

        Assert.Equal(ExitCode.Success, exitCode);
        Assert.Equal(expected, WrittenReportNames());
    }

    [Fact]
    public async Task RunAsync_WhitespaceReport_WritesAllReports()
    {
        InMemorySaleRepository repository = new(SalesFor("frankfurt", "berlin"));
        string[] expected = ["berlin", "frankfurt"];

        ExitCode exitCode = await RunAsync(repository, report: "  ");

        Assert.Equal(ExitCode.Success, exitCode);
        Assert.Equal(expected, WrittenReportNames());
    }

    [Fact]
    public async Task RunAsync_WithReport_WritesOnlyThatReport()
    {
        InMemorySaleRepository repository = new(SalesFor("frankfurt", "berlin"));
        MonthlySalesReport[] expected = [new MonthlySalesReport("berlin", Year, Month, 100m)];

        ExitCode exitCode = await RunAsync(repository, report: "berlin");

        Assert.Equal(ExitCode.Success, exitCode);
        Assert.Equal(expected, _writer.WrittenReports);
    }

    [Fact]
    public async Task RunAsync_NoReportsInFolder_ReturnsNoReportsFound()
    {
        InMemorySaleRepository repository = new(SalesFor());

        ExitCode exitCode = await RunAsync(repository, report: null);

        Assert.Equal(ExitCode.NoReportsFound, exitCode);
        Assert.Empty(_writer.WrittenReports);
    }

    [Fact]
    public async Task RunAsync_UnknownReport_ReturnsReportNotFound()
    {
        InMemorySaleRepository repository = new(SalesFor("berlin"));

        ExitCode exitCode = await RunAsync(repository, report: "nope");

        Assert.Equal(ExitCode.ReportNotFound, exitCode);
        Assert.Empty(_writer.WrittenReports);
    }

    [Fact]
    public async Task RunAsync_InvalidReport_IsSkippedAndOthersAreWritten()
    {
        InMemorySaleRepository repository = new(SalesFor("berlin", "muenchen")) { InvalidReportNames = ["hamburg"] };
        string[] expected = ["berlin", "muenchen"];

        ExitCode exitCode = await RunAsync(repository, report: null);

        Assert.Equal(ExitCode.InvalidReportData, exitCode);
        Assert.Equal(expected, WrittenReportNames());
    }

    private Task<ExitCode> RunAsync(InMemorySaleRepository repository, string? report)
    {
        ReportRequestOptions request = new() { Year = Year, Month = Month, Report = report };
        ReportJob job = new(Options.Create(request), repository, new ReportService(repository), _writer, NullLogger<ReportJob>.Instance);

        return job.RunAsync(CancellationToken.None);
    }

    private IEnumerable<string> WrittenReportNames() => _writer.WrittenReports.Select(report => report.ReportName);

    private static Dictionary<string, IReadOnlyList<Sale>> SalesFor(params string[] reportNames) =>
        reportNames.ToDictionary(reportName => reportName, _ => OctoberSales);
}
