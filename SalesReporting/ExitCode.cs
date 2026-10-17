namespace SalesReporting;

public enum ExitCode
{
    Success = 0,
    NoReportsFound = 1,
    ReportNotFound = 2,
    InvalidReportData = 4,
    InvalidArguments = 8
}
