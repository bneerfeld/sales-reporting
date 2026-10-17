using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SalesReporting.Domain;
using SalesReporting.Persistence;

namespace SalesReporting.Reporting;

public sealed class ReportJob(
    IOptions<ReportRequestOptions> requestOptions,
    ISaleRepository saleRepository,
    ReportService reportService,
    IReportWriter reportWriter,
    ILogger<ReportJob> logger)
{
    public async Task<ExitCode> RunAsync(CancellationToken cancellationToken)
    {
        ReportRequestOptions request = requestOptions.Value;

        IReadOnlyList<string> reportNames = string.IsNullOrWhiteSpace(request.Report) ? saleRepository.GetReportNames() : [request.Report];

        if (reportNames.Count == 0)
        {
            logger.LogError("Im Datenordner wurden keine Reports gefunden.");
            return ExitCode.NoReportsFound;
        }

        ExitCode exitCode = ExitCode.Success;

        foreach (string reportName in reportNames)
        {
            try
            {
                MonthlySalesReport? report = await reportService.CreateAsync(request.Year, request.Month, reportName, cancellationToken);

                if (report is null)
                {
                    logger.LogError("Report {ReportName} wurde im Datenordner nicht gefunden.", reportName);
                    exitCode = ExitCode.ReportNotFound;
                    continue;
                }

                logger.LogInformation("Monatlicher Verkaufsbericht {ReportName} {Month:D2}/{Year}: Gesamtumsatz {TotalUsd:N2} USD", report.ReportName, report.Month, report.Year, report.TotalUsd);

                string filePath = await reportWriter.WriteAsync(report, cancellationToken);
                logger.LogInformation("Report {ReportName} wurde nach {FilePath} geschrieben.", reportName, filePath);
            }
            catch (JsonException exception)
            {
                string error = exception.InnerException is null ? exception.Message : $"{exception.Message} {exception.InnerException.Message}";
                logger.LogError("Report {ReportName} enthält ungültige Daten und wurde übersprungen: {Error}", reportName, error);
                exitCode = ExitCode.InvalidReportData;
            }
        }

        return exitCode;
    }
}
