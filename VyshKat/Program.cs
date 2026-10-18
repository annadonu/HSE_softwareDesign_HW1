// точка входа: регистрирует зависимости в DI-контейнере и запускает меню.
using Microsoft.Extensions.DependencyInjection;
using Vyshkat.Demo;
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Services;

namespace Vyshkat;

internal static class Program
{
    internal static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IServiceCenter, ServiceCenter>();
        services.AddSingleton<InventoryNumberGenerator>();
        services.AddSingleton<Fleet>();
        services.AddSingleton<FleetReportService>();
        services.AddSingleton<ConsolePrinter>();
        services.AddSingleton<ConsoleReader>();
        services.AddSingleton<Demonstration>();
        services.AddSingleton<MainMenu>();
    }

    internal static void Main()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<MainMenu>().Run();
    }
}