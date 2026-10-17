namespace SalesReporting.Reporting;

public sealed class ReportRequestOptions
{
    public const int FirstMonth = 1;
    public const int LastMonth = 12;

    public int Year { get; set; }

    public int Month { get; set; }

    public string? Report { get; set; }
}
