using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public sealed class JsonSaleRepository(SalesFileLocator salesFileLocator) : ISaleRepository
{
    public IReadOnlyList<string> GetReportNames() => salesFileLocator.ListReportNames();

    public async Task<IReadOnlyList<Sale>?> GetAllAsync(string reportName, CancellationToken cancellationToken)
    {
        string? filePath = salesFileLocator.Locate(reportName);

        if (filePath is null)
            return null;

        await using FileStream stream = File.OpenRead(filePath);

        return await SaleJsonParser.ParseAsync(stream, cancellationToken);
    }
}
