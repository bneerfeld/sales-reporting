using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Reporting;

public sealed class ReportService(ISaleRepository saleRepository)
{
    public async Task<MonthlySalesReport?> CreateAsync(int year, int month, string reportName, CancellationToken cancellationToken)
    {
        IReadOnlyList<Sale>? sales = await saleRepository.GetAllAsync(reportName, cancellationToken);

        if (sales is null)
            return null;

        decimal totalUsd = sales
            .Where(sale => sale.Date.Year == year && sale.Date.Month == month)
            .Sum(sale => sale.AmountInUsd);

        return new MonthlySalesReport(reportName, year, month, totalUsd);
    }
}