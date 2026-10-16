using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SalesReporting.Persistence;
using SalesReporting.Reporting;
using Serilog;

namespace SalesReporting;

public static class DependencyInjection
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(serviceProvider));

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SalesFileOptions>()
            .Bind(configuration.GetSection(SalesFileOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.FilePath), $"'{SalesFileOptions.SectionName}:FilePath' must be configured.");

        services.AddSingleton<ISaleRepository>(serviceProvider =>
        {
            SalesFileOptions options = serviceProvider.GetRequiredService<IOptions<SalesFileOptions>>().Value;
            IHostEnvironment environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            return new JsonSaleRepository(Path.Combine(environment.ContentRootPath, options.FilePath));
        });

        return services;
    }

    public static IServiceCollection AddReporting(this IServiceCollection services)
    {
        services.AddSingleton<ReportService>();
        services.AddSingleton<ReportPrinter>();

        return services;
    }
}