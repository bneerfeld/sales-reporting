using System.Text.Json;
using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Tests.Persistence;

public sealed class JsonReportWriterTests : IDisposable
{
    private readonly string _rootFolder = Directory.CreateTempSubdirectory().FullName;
    private readonly string _outputFolder;
    private readonly JsonReportWriter _writer;

    public JsonReportWriterTests()
    {
        _outputFolder = Path.Combine(_rootFolder, "Reports");
        _writer = new JsonReportWriter(_outputFolder);
    }

    public void Dispose() => Directory.Delete(_rootFolder, recursive: true);

    [Fact]
    public async Task WriteAsync_MissingOutputFolder_IsCreated()
    {
        await _writer.WriteAsync(new MonthlySalesReport("berlin", 2026, 10, 100m), CancellationToken.None);

        Assert.True(Directory.Exists(_outputFolder));
    }

    [Fact]
    public async Task WriteAsync_NamesFileByReportYearAndPaddedMonth()
    {
        string filePath = await _writer.WriteAsync(new MonthlySalesReport("berlin", 2026, 3, 100m), CancellationToken.None);

        Assert.Equal(Path.Combine(_outputFolder, "berlin_2026_03.json"), filePath);
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task WriteAsync_WritesReportAsCamelCaseJson()
    {
        string filePath = await _writer.WriteAsync(new MonthlySalesReport("berlin", 2026, 10, 1234.56m), CancellationToken.None);

        JsonElement json = await ReadJsonAsync(filePath);

        Assert.Equal("berlin", json.GetProperty("reportName").GetString());
        Assert.Equal(2026, json.GetProperty("year").GetInt32());
        Assert.Equal(10, json.GetProperty("month").GetInt32());
        Assert.Equal(1234.56m, json.GetProperty("totalUsd").GetDecimal());
    }

    [Fact]
    public async Task WriteAsync_ExistingReport_IsOverwritten()
    {
        await _writer.WriteAsync(new MonthlySalesReport("berlin", 2026, 10, 1234.56m), CancellationToken.None);
        string filePath = await _writer.WriteAsync(new MonthlySalesReport("berlin", 2026, 10, 1m), CancellationToken.None);

        JsonElement json = await ReadJsonAsync(filePath);

        Assert.Equal(1m, json.GetProperty("totalUsd").GetDecimal());
    }

    private static async Task<JsonElement> ReadJsonAsync(string filePath) =>
        JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(filePath));
}
