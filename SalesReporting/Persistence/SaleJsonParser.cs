using System.Text.Json;
using Ardalis.SmartEnum.SystemTextJson;
using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public static class SaleJsonParser
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
        Converters = { new SmartEnumNameConverter<Currency, int>() }
    };

    public static async Task<IReadOnlyList<Sale>> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        List<Sale>? sales = await JsonSerializer.DeserializeAsync<List<Sale>>(stream, SerializerOptions, cancellationToken);

        return sales ?? [];
    }
}
