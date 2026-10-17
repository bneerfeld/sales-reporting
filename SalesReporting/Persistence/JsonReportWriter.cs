using System.Text.Json;
using SalesReporting.Domain;

namespace SalesReporting.Persistence;

public sealed class JsonReportWriter(string outputFolder) : IReportWriter
{
    private const string FileExtension = ".json";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public async Task<string> WriteAsync(MonthlySalesReport report, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputFolder);

        string filePath = Path.Combine(outputFolder, $"{report.ReportName}_{report.Year}_{report.Month:D2}{FileExtension}");

        await using FileStream stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, report, SerializerOptions, cancellationToken);

        return filePath;
    }
}
