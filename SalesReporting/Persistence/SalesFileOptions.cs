namespace SalesReporting.Persistence;

public sealed class SalesFileOptions
{
    public const string SectionName = "SalesFile";

    public string FilePath { get; init; } = string.Empty;
}
