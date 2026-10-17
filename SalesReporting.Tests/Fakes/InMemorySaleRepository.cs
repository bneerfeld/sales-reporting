using System.Text.Json;
using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Tests.Fakes;

public sealed class InMemorySaleRepository(IReadOnlyDictionary<string, IReadOnlyList<Sale>> salesByReport) : ISaleRepository
{
    public IReadOnlyCollection<string> InvalidReportNames { get; init; } = [];

    public IReadOnlyList<string> GetReportNames() => [.. salesByReport.Keys.Concat(InvalidReportNames).Order()];

    public Task<IReadOnlyList<Sale>?> GetAllAsync(string reportName, CancellationToken cancellationToken)
    {
        if (InvalidReportNames.Contains(reportName))
            return Task.FromException<IReadOnlyList<Sale>?>(new JsonException($"Report {reportName} contains invalid data."));

        return Task.FromResult(salesByReport.GetValueOrDefault(reportName));
    }
}
