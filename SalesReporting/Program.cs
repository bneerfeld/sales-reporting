using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesReporting;
using SalesReporting.Domain;
using SalesReporting.Reporting;

const string YearKey = "Year";
const string MonthKey = "Month";

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services
    .AddSerilogLogging(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddReporting();

using IHost host = builder.Build();

int year = builder.Configuration.GetValue<int>(YearKey);
int month = builder.Configuration.GetValue<int>(MonthKey);

ReportService reportService = host.Services.GetRequiredService<ReportService>();
ReportPrinter reportPrinter = host.Services.GetRequiredService<ReportPrinter>();

MonthlySalesReport report = await reportService.CreateAsync(year, month, CancellationToken.None);
reportPrinter.Print(report);