namespace SalesReporting.Persistence;

public sealed class FolderOptions
{
    public const string SectionName = "Folders";

    public string Sales { get; init; } = string.Empty;

    public string Reports { get; init; } = string.Empty;
}
