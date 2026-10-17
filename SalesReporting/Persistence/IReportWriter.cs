using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public interface IReportWriter
{
    Task<string> WriteAsync(MonthlySalesReport report, CancellationToken cancellationToken);
}
