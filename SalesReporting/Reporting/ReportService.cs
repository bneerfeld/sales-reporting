using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Reporting;

public sealed class ReportService(ISaleRepository saleRepository)
{
    private const int FirstMonth = 1;
    private const int LastMonth = 12;

    public async Task<MonthlySalesReport> CreateAsync(int year, int month, CancellationToken cancellationToken)
    {
        if (month is < FirstMonth or > LastMonth)
            throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be between 1 and 12.");

        IReadOnlyList<Sale> sales = await saleRepository.GetAllAsync(cancellationToken);

        decimal totalUsd = sales
            .Where(sale => sale.Date.Year == year && sale.Date.Month == month)
            .Sum(sale => sale.AmountInUsd);

        return new MonthlySalesReport(year, month, totalUsd);
    }
}