namespace SalesReporting.Persistence;

public sealed class SalesFileLocator(string salesFolder)
{
    private const string FileExtension = ".json";

    private readonly string _salesFolder = Path.GetFullPath(salesFolder);

    public string? Locate(string reportName)
    {
        string filePath = Path.GetFullPath(Path.Combine(_salesFolder, reportName + FileExtension));

        bool isInsideSalesFolder = Path.GetDirectoryName(filePath) == _salesFolder;

        return isInsideSalesFolder && File.Exists(filePath) ? filePath : null;
    }

    public IReadOnlyList<string> ListReportNames()
    {
        if (!Directory.Exists(_salesFolder))
            return [];

        return
        [
            .. Directory.EnumerateFiles(_salesFolder, "*" + FileExtension)
                .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                .Order(StringComparer.Ordinal)
        ];
    }
}
