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
        services.AddOptions<FolderOptions>()
            .Bind(configuration.GetSection(FolderOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Sales), $"'{FolderOptions.SectionName}:Sales' must be configured.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Reports), $"'{FolderOptions.SectionName}:Reports' must be configured.");

        services.AddSingleton(serviceProvider =>
        {
            FolderOptions options = serviceProvider.GetRequiredService<IOptions<FolderOptions>>().Value;
            IHostEnvironment environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            return new SalesFileLocator(Path.Combine(environment.ContentRootPath, options.Sales));
        });

        services.AddSingleton<IReportWriter>(serviceProvider =>
        {
            FolderOptions options = serviceProvider.GetRequiredService<IOptions<FolderOptions>>().Value;
            IHostEnvironment environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            return new JsonReportWriter(Path.Combine(environment.ContentRootPath, options.Reports));
        });

        services.AddSingleton<ISaleRepository, JsonSaleRepository>();

        return services;
    }

    public static IServiceCollection AddReporting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ReportRequestOptions>()
            .Bind(configuration)
            .Validate(options => options.Year >= DateTime.MinValue.Year && options.Year <= DateTime.MaxValue.Year, "'--Year' must be a valid year.")
            .Validate(options => options.Month is >= ReportRequestOptions.FirstMonth and <= ReportRequestOptions.LastMonth, $"'--Month' must be between {ReportRequestOptions.FirstMonth} and {ReportRequestOptions.LastMonth}.");

        services.AddSingleton<ReportService>();
        services.AddSingleton<ReportJob>();

        return services;
    }
}
