using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SalesReporting;
using SalesReporting.Reporting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services
    .AddSerilogLogging(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddReporting(builder.Configuration);

using IHost host = builder.Build();

try
{
    ReportJob reportJob = host.Services.GetRequiredService<ReportJob>();

    return (int) await reportJob.RunAsync(CancellationToken.None);
}
catch (OptionsValidationException exception)
{
    ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError("Ungültige Konfiguration: {Error}", exception.Message);
    return (int) ExitCode.InvalidArguments;
}
