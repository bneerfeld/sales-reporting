namespace SalesReporting.Reporting;

public sealed class ReportRequestOptions
{
    public const int FirstMonth = 1;
    public const int LastMonth = 12;

    public int Year { get; init; }

    public int Month { get; init; }

    public string? Report { get; init; }
}
