// тесты DI: подмена техосмотра через контейнер, один парк на всех и сборка графа как в Program.cs.
using Microsoft.Extensions.DependencyInjection;
using Vyshkat;
using Vyshkat.Demo;
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Models.Transports;
using Vyshkat.Services;
using Xunit;

namespace Vyshkat.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void Fleet_ResolvedFromContainer_UsesSubstitutedServiceCenter()
    {
        var services = new ServiceCollection();
        Program.ConfigureServices(services);

        services.AddSingleton<IServiceCenter>(
            TestFactory.RejectingServiceCenter("Vehicle wear"));

        using var provider = services.BuildServiceProvider();

        var fleet = provider.GetRequiredService<Fleet>();
        var result = fleet.Add(
            new Bicycle("BI-001", "Bicycle", 6, 80));

        Assert.Equal("Vehicle wear", result);
        Assert.Empty(fleet.Items);
    }

    [Fact]
    public void Fleet_ResolvedTwice_ReturnsSameInstance()
    {
        using var provider = BuildRealProvider();

        Assert.Same(
            provider.GetRequiredService<Fleet>(),
            provider.GetRequiredService<Fleet>());
    }

    [Fact]
    public void ReportService_SeesItemsAddedToResolvedFleet()
    {
        using var provider = BuildRealProvider();

        var fleet = provider.GetRequiredService<Fleet>();
        var reports = provider.GetRequiredService<FleetReportService>();

        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));

        Assert.Equal(1, reports.GetTotalCount());
    }

    [Fact]
    public void RealContainer_UsesRealServiceCenter()
    {
        using var provider = BuildRealProvider();

        var fleet = provider.GetRequiredService<Fleet>();

        var rejected = fleet.Add(
            new Bicycle("BI-001", "Worn bicycle", 6, 49));

        var accepted = fleet.Add(
            new Bicycle("BI-002", "Good bicycle", 6, 50));

        Assert.Equal(ServiceCenter.WearReason, rejected);
        Assert.Null(accepted);
        Assert.Single(fleet.Items);
    }

    [Fact]
    public void ConfiguredContainer_ResolvesMainMenu()
    {
        using var provider = BuildRealProvider();

        var menu = provider.GetRequiredService<MainMenu>();

        Assert.NotNull(menu);
    }

    private static ServiceProvider BuildRealProvider()
    {
        var services = new ServiceCollection();
        Program.ConfigureServices(services);

        return services.BuildServiceProvider();
    }
}