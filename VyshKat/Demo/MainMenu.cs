// главное меню: читает выбор пользователя и вызывает парк, отчеты и готовый сценарий.
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Models.Things;
using Vyshkat.Models.Transports;
using Vyshkat.Services;

namespace Vyshkat.Demo;

internal sealed class MainMenu(
    Fleet fleet,
    FleetReportService reports,
    InventoryNumberGenerator generator,
    ConsolePrinter printer,
    ConsoleReader reader,
    Demonstration demonstration)
{
    public void Run()
    {
        try
        {
            while (true)
            {
                ShowMenu();
                var choice = reader.ReadInt("Choose an option", 0, 7);
                if (choice == 0)
                    return;

                Execute(choice);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void ShowMenu()
    {
        printer.Title("VyshKat: fleet accounting");
        printer.Info("1. Accept transport");
        printer.Info("2. Accept a thing");
        printer.Info("3. List of items");
        printer.Info("4. Number of items");
        printer.Info("5. Total energy consumption");
        printer.Info("6. Suitable for novices");
        printer.Info("7. Run the ready-made scenario");
        printer.Info("0. Exit");
    }

    private void Execute(int choice)
    {
        switch (choice)
        {
            case 1: AddTransport(); break;
            case 2: AddThing(); break;
            case 3: ShowInventory(); break;
            case 4: ShowCounts(); break;
            case 5: ShowEnergy(); break;
            case 6: ShowNoviceFriendly(); break;
            case 7: demonstration.Run(); break;
        }
    }

    private void AddTransport()
    {
        printer.Title("Accepting transport");
        var kind = reader.ReadInt("Kind (1 - bicycle, 2 - electric bicycle, 3 - electric scooter)", 1, 3);
        var name = reader.ReadText("Name");
        var easeOfUse = reader.ReadInt("Ease of use for a novice (1-10)", 1, 10);
        var conditionScore = reader.ReadInt("Condition score (0-100)", 0, 100);
        var energy = kind != 1 ? reader.ReadDouble("Daily consumption, kWh", 0, 1000) : 0;
        var speed = kind == 3 ? reader.ReadInt("Maximum speed, km/h", 1, 100) : 0;

        Register(() => kind switch
        {
            1 => new Bicycle(Next(Bicycle.Prefix), name, easeOfUse, conditionScore),
            2 => new ElectricBicycle(Next(ElectricBicycle.Prefix), name, easeOfUse, conditionScore, energy),
            _ => new ElectricScooter(Next(ElectricScooter.Prefix), name, easeOfUse, conditionScore, energy, speed)
        });
    }

    private void AddThing()
    {
        printer.Title("Accepting a thing");
        var kind = reader.ReadInt("Kind (1 - helmet, 2 - dock station, 3 - charging cabinet)", 1, 3);
        var name = reader.ReadText("Name");
        var energy = kind != 1 ? reader.ReadDouble("Daily consumption, kWh", 0, 1000) : 0;

        Register(() => kind switch
        {
            1 => new Helmet(Next(Helmet.Prefix), name),
            2 => new DockStation(Next(DockStation.Prefix), name, energy),
            _ => new ChargingCabinet(Next(ChargingCabinet.Prefix), name, energy)
        });
    }

    private void ShowInventory()
    {
        printer.Title("Items on balance");
        printer.Items(reports.GetInventoryList());
    }

    private void ShowCounts()
    {
        printer.Title("Number of items");
        printer.Info($"Total on balance: {reports.GetTotalCount()}");
        foreach (var (kind, count) in reports.CountByKind())
            printer.Info($"  {kind}: {count}");
    }

    private void ShowEnergy()
    {
        printer.Title("Energy consumption");
        printer.Info($"Total daily consumption: {printer.FormatKwh(reports.GetTotalDailyEnergyKwh())}");
    }

    private void ShowNoviceFriendly()
    {
        printer.Title("Suitable for novices");
        printer.Items(reports.GetNoviceFriendlyItems());
    }

    private string Next(string prefix) => generator.Next(prefix, fleet.Items);

    private void Register(Func<IInventoryItem> create)
    {
        try
        {
            var item = create();
            printer.AddResult(item, fleet.Add(item));
        }
        catch (ArgumentException ex)
        {
            printer.Error(ex.Message);
        }
    }
}