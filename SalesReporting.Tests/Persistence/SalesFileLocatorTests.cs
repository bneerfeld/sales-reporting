using SalesReporting.Persistence;

namespace SalesReporting.Tests.Persistence;

public sealed class SalesFileLocatorTests : IDisposable
{
    private readonly string _rootFolder = Directory.CreateTempSubdirectory().FullName;
    private readonly string _salesFolder;
    private readonly SalesFileLocator _locator;

    public SalesFileLocatorTests()
    {
        _salesFolder = Directory.CreateDirectory(Path.Combine(_rootFolder, "Data")).FullName;
        _locator = new SalesFileLocator(_salesFolder);
    }

    public void Dispose() => Directory.Delete(_rootFolder, recursive: true);

    [Fact]
    public void Locate_ExistingReport_ReturnsFilePath()
    {
        string filePath = CreateFile(_salesFolder, "berlin.json");

        Assert.Equal(filePath, _locator.Locate("berlin"));
    }

    [Fact]
    public void Locate_MissingReport_ReturnsNull()
    {
        Assert.Null(_locator.Locate("berlin"));
    }

    [Fact]
    public void Locate_FileOutsideSalesFolder_ReturnsNull()
    {
        CreateFile(_rootFolder, "secret.json");

        Assert.Null(_locator.Locate("../secret"));
    }

    [Fact]
    public void Locate_FileInSubfolder_ReturnsNull()
    {
        CreateFile(Directory.CreateDirectory(Path.Combine(_salesFolder, "archive")).FullName, "berlin.json");

        Assert.Null(_locator.Locate("archive/berlin"));
    }

    [Fact]
    public void ListReportNames_ReturnsSortedJsonFileNamesWithoutExtension()
    {
        CreateFile(_salesFolder, "frankfurt.json");
        CreateFile(_salesFolder, "berlin.json");
        CreateFile(_salesFolder, "notes.txt");
        string[] expected = ["berlin", "frankfurt"];

        Assert.Equal(expected, _locator.ListReportNames());
    }

    [Fact]
    public void ListReportNames_IgnoresSubfolders()
    {
        CreateFile(_salesFolder, "berlin.json");
        CreateFile(Directory.CreateDirectory(Path.Combine(_salesFolder, "archive")).FullName, "frankfurt.json");
        string[] expected = ["berlin"];

        Assert.Equal(expected, _locator.ListReportNames());
    }

    [Fact]
    public void ListReportNames_MissingFolder_ReturnsEmpty()
    {
        SalesFileLocator locator = new(Path.Combine(_rootFolder, "Missing"));

        Assert.Empty(locator.ListReportNames());
    }

    private static string CreateFile(string folder, string fileName)
    {
        string filePath = Path.Combine(folder, fileName);
        File.WriteAllText(filePath, "[]");

        return filePath;
    }
}
