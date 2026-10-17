using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public interface ISaleRepository
{
    IReadOnlyList<string> GetReportNames();

    Task<IReadOnlyList<Sale>?> GetAllAsync(string reportName, CancellationToken cancellationToken);
}
