using SalesReporting.Domain;
using SalesReporting.Reporting;
using SalesReporting.Tests.Fakes;

namespace SalesReporting.Tests.Reporting;

public sealed class ReportServiceTests
{
    private const string ReportName = "berlin";
    private const int Year = 2026;
    private const int Month = 10;

    public static TheoryData<DateTime> DatesInsideMonth => new()
    {
        new DateTime(2026, 10, 1, 0, 0, 0),
        new DateTime(2026, 10, 15),
        new DateTime(2026, 10, 31, 23, 59, 59)
    };

    public static TheoryData<DateTime> DatesOutsideMonth => new()
    {
        new DateTime(2026, 9, 30, 23, 59, 59),
        new DateTime(2026, 11, 1, 0, 0, 0),
        new DateTime(2025, 10, 15),
        new DateTime(2027, 10, 15)
    };

    [Theory]
    [MemberData(nameof(DatesInsideMonth))]
    public async Task CreateAsync_SaleInsideMonth_IsCounted(DateTime date)
    {
        MonthlySalesReport? report = await CreateReportAsync(Year, Month, new Sale(date, 100m, Currency.Usd));

        Assert.Equal(100m, report?.TotalUsd);
    }

    [Theory]
    [MemberData(nameof(DatesOutsideMonth))]
    public async Task CreateAsync_SaleOutsideMonth_IsIgnored(DateTime date)
    {
        MonthlySalesReport? report = await CreateReportAsync(Year, Month, new Sale(date, 100m, Currency.Usd));

        Assert.Equal(0m, report?.TotalUsd);
    }

    [Fact]
    public async Task CreateAsync_LeapDay_IsCountedForFebruary()
    {
        MonthlySalesReport? report = await CreateReportAsync(2028, 2, new Sale(new DateTime(2028, 2, 29), 100m, Currency.Usd));

        Assert.Equal(100m, report?.TotalUsd);
    }

    [Fact]
    public async Task CreateAsync_ConvertsEachSaleToUsdBeforeSumming()
    {
        MonthlySalesReport? report = await CreateReportAsync(Year, Month,
            new Sale(new DateTime(2026, 10, 1), 100m, Currency.Usd),
            new Sale(new DateTime(2026, 10, 2), 100m, Currency.Eur),
            new Sale(new DateTime(2026, 10, 3), 100m, Currency.Gbp));

        Assert.Equal(340m, report?.TotalUsd);
    }

    [Fact]
    public async Task CreateAsync_SumsWithoutPrecisionLoss()
    {
        MonthlySalesReport? report = await CreateReportAsync(Year, Month,
            new Sale(new DateTime(2026, 10, 1), 0.1m, Currency.Eur),
            new Sale(new DateTime(2026, 10, 2), 0.1m, Currency.Eur),
            new Sale(new DateTime(2026, 10, 3), 0.1m, Currency.Eur));

        Assert.Equal(0.33m, report?.TotalUsd);
    }

    [Fact]
    public async Task CreateAsync_NoSalesInMonth_ReturnsReportWithZeroTotal()
    {
        MonthlySalesReport? report = await CreateReportAsync(Year, Month, new Sale(new DateTime(2026, 9, 15), 100m, Currency.Usd));

        Assert.Equal(new MonthlySalesReport(ReportName, Year, Month, 0m), report);
    }

    [Fact]
    public async Task CreateAsync_UnknownReport_ReturnsNull()
    {
        ReportService service = new(new InMemorySaleRepository(new Dictionary<string, IReadOnlyList<Sale>>()));

        MonthlySalesReport? report = await service.CreateAsync(Year, Month, ReportName, CancellationToken.None);

        Assert.Null(report);
    }

    private static Task<MonthlySalesReport?> CreateReportAsync(int year, int month, params Sale[] sales)
    {
        InMemorySaleRepository repository = new(new Dictionary<string, IReadOnlyList<Sale>> { [ReportName] = sales });
        ReportService service = new(repository);

        return service.CreateAsync(year, month, ReportName, CancellationToken.None);
    }
}
