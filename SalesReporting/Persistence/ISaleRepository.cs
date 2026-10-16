using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public interface ISaleRepository
{
    Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken);
}