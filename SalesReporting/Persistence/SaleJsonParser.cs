using System.Text.Json;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public static class SaleJsonParser
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        NumberHandling = JsonNumberHandling.Strict,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
        Converters = { new SmartEnumNameConverter<Currency, int>() }
    };

    public static async Task<IReadOnlyList<Sale>> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        List<Sale?>? sales = await JsonSerializer.DeserializeAsync<List<Sale?>>(stream, SerializerOptions, cancellationToken);

        if (sales is null || sales.Contains(null))
            throw new JsonException("Sales data must be an array without empty entries.");

        return [.. sales.OfType<Sale>()];
    }
}
