using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SalesReporting.Reporting;

namespace SalesReporting.Tests.Reporting;

public sealed class ReportRequestValidationTests
{
    [Fact]
    public void ValidArguments_AreBound()
    {
        ReportRequestOptions options = Resolve("--Year 2026 --Month 10 --Report berlin");

        Assert.Equal(2026, options.Year);
        Assert.Equal(10, options.Month);
        Assert.Equal("berlin", options.Report);
    }

    [Fact]
    public void WithoutReport_ReportIsNull()
    {
        ReportRequestOptions options = Resolve("--Year 2026 --Month 10");

        Assert.Null(options.Report);
    }

    [Theory]
    [InlineData("--Year 1 --Month 1")]
    [InlineData("--Year 9999 --Month 12")]
    public void BoundaryValues_AreAccepted(string arguments)
    {
        Exception? exception = Record.Exception(() => Resolve(arguments));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData("--Month 10")]
    [InlineData("--Year 2026")]
    public void MissingArgument_FailsValidation(string arguments)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(arguments));
    }

    [Theory]
    [InlineData("--Month 10 --Year")]
    [InlineData("--Year --Month 10")]
    public void MissingValue_FailsValidation(string arguments)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(arguments));
    }

    [Theory]
    [InlineData("--Year 2026 --Month 0")]
    [InlineData("--Year 2026 --Month 13")]
    [InlineData("--Year 0 --Month 10")]
    [InlineData("--Year 10000 --Month 10")]
    [InlineData("--Year -2026 --Month 10")]
    public void OutOfRangeValue_FailsValidation(string arguments)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(arguments));
    }

    [Theory]
    [InlineData("--Year abc --Month 10")]
    [InlineData("--Year 2026 --Month Oktober")]
    public void NonNumericValue_FailsValidation(string arguments)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(arguments));
    }

    [Theory]
    [InlineData("-Year 2026 --Month 10")]
    [InlineData("--Yaer 2026 --Month 10")]
    public void MistypedSwitch_FailsValidation(string arguments)
    {
        Assert.Throws<OptionsValidationException>(() => Resolve(arguments));
    }

    [Fact]
    public void ReportNameWithPath_FailsValidation()
    {
        Assert.Throws<OptionsValidationException>(() => Resolve("--Year 2026 --Month 10 --Report ../Data/berlin"));
    }

    private static ReportRequestOptions Resolve(string arguments)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddCommandLine(arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Build();

        using ServiceProvider provider = new ServiceCollection()
            .AddReporting(configuration)
            .BuildServiceProvider();

        return provider.GetRequiredService<IOptions<ReportRequestOptions>>().Value;
    }
}
