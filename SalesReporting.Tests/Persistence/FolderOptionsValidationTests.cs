using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SalesReporting.Persistence;

namespace SalesReporting.Tests.Persistence;

public sealed class FolderOptionsValidationTests
{
    [Fact]
    public void ConfiguredFolders_AreBound()
    {
        FolderOptions options = Resolve("Data", "Reports");

        Assert.Equal("Data", options.Sales);
        Assert.Equal("Reports", options.Reports);
    }

    [Theory]
    [InlineData(null, "Reports")]
    [InlineData("", "Reports")]
    [InlineData(" ", "Reports")]
    [InlineData("Data", null)]
    [InlineData("Data", "")]
    [InlineData("Data", " ")]
    public void MissingFolder_FailsValidation(string? sales, string? reports)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(sales, reports));
    }

    private static FolderOptions Resolve(string? sales, string? reports)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Folders:Sales"] = sales,
                ["Folders:Reports"] = reports
            })
            .Build();

        using ServiceProvider provider = new ServiceCollection()
            .AddPersistence(configuration)
            .BuildServiceProvider();

        return provider.GetRequiredService<IOptions<FolderOptions>>().Value;
    }
}
