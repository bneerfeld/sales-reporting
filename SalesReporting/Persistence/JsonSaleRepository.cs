using System.Text.Json;
using Ardalis.SmartEnum.SystemTextJson;
using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public sealed class JsonSaleRepository(string filePath) : ISaleRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new SmartEnumNameConverter<Currency, int>() }
    };

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using FileStream stream = File.OpenRead(filePath);

        List<Sale>? sales = await JsonSerializer.DeserializeAsync<List<Sale>>(stream, SerializerOptions, cancellationToken);

        return sales ?? [];
    }
}