using System.Text;
using System.Text.Json;
using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Tests.Persistence;

public sealed class SaleJsonParserTests
{
    [Fact]
    public async Task ParseAsync_ValidContent_ReturnsSales()
    {
        const string json = """
            [
              {"date": "2026-10-01", "amount": 12.34, "currency": "EUR"},
              {"date": "2026-10-31", "amount": 100, "currency": "GBP"}
            ]
            """;
        Sale[] expected =
        [
            new Sale(new DateOnly(2026, 10, 1), 12.34m, Currency.Eur),
            new Sale(new DateOnly(2026, 10, 31), 100m, Currency.Gbp)
        ];

        IReadOnlyList<Sale> sales = await ParseAsync(json);

        Assert.Equal(expected, sales);
    }

    [Fact]
    public async Task ParseAsync_EmptyArray_ReturnsNoSales()
    {
        IReadOnlyList<Sale> sales = await ParseAsync("[]");

        Assert.Empty(sales);
    }

    [Theory]
    [InlineData("")]
    [InlineData("""{"date": "2026-10-01", "amount": 10, "currency": "USD"}""")]
    [InlineData("""[{"date": "2026-10-01", "amount": 10, "currency": "USD"}""")]
    public async Task ParseAsync_MalformedJson_ThrowsJsonException(string json)
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Theory]
    [InlineData("""[{"amount": 10, "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": 10}]""")]
    public async Task ParseAsync_MissingProperty_ThrowsJsonException(string json)
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Theory]
    [InlineData("""[{"date": null, "amount": 10, "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": null, "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": 10, "currency": null}]""")]
    public async Task ParseAsync_NullValue_ThrowsJsonException(string json)
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Theory]
    [InlineData("""[{"date": "2026-13-01", "amount": 10, "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": "10", "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": "abc", "currency": "USD"}]""")]
    [InlineData("""[{"date": "2026-10-01", "amount": true, "currency": "USD"}]""")]
    public async Task ParseAsync_InvalidValue_ThrowsJsonException(string json)
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Fact]
    public async Task ParseAsync_TimestampWithOffset_ThrowsJsonException()
    {
        const string json = """[{"date": "2026-10-01T00:30:00+02:00", "amount": 10, "currency": "USD"}]""";

        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Fact]
    public async Task ParseAsync_UnknownCurrency_ThrowsJsonException()
    {
        const string json = """[{"date": "2026-10-01", "amount": 10, "currency": "CHF"}]""";

        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Fact]
    public async Task ParseAsync_LowercaseCurrency_ThrowsJsonException()
    {
        const string json = """[{"date": "2026-10-01", "amount": 10, "currency": "eur"}]""";

        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync(json));
    }

    [Fact]
    public async Task ParseAsync_NullEntry_ThrowsJsonException()
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync("[null]"));
    }

    [Fact]
    public async Task ParseAsync_NullLiteral_ThrowsJsonException()
    {
        await Assert.ThrowsAnyAsync<JsonException>(() => ParseAsync("null"));
    }

    private static async Task<IReadOnlyList<Sale>> ParseAsync(string json)
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));

        return await SaleJsonParser.ParseAsync(stream, CancellationToken.None);
    }
}
