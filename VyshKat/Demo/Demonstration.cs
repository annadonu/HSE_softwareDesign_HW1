// готовый сценарий: показывает приемку, техосмотр, границы правил, ошибки и все отчеты.
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Models.Things;
using Vyshkat.Models.Transports;
using Vyshkat.Services;

namespace Vyshkat.Demo;

internal sealed class Demonstration(
    Fleet fleet,
    FleetReportService reports,
    InventoryNumberGenerator generator,
    ConsolePrinter printer)
{
    public void Run()
    {
        printer.Title("Ready-made scenario");
        printer.Info("Items are added to the current fleet, numbers are issued by the generator.");
        printer.Pause();

        AcceptTransport();
        printer.Pause();

        AcceptThings();
        printer.Pause();

        ShowErrors();
        printer.Pause();

        ShowReports();
    }

    private void AcceptTransport()
    {
        printer.Title("1. Accepting transport and inspection");

        Add(() => new ElectricScooter(Next(ElectricScooter.Prefix), "City scooter", 8, 80, 0.35, 25));
        Add(() => new ElectricScooter(Next(ElectricScooter.Prefix), "Fast scooter", 8, 90, 0.35, 30));
        Add(() => new ElectricScooter(Next(ElectricScooter.Prefix), "Worn scooter", 8, 40, 0.35, 25));
        Add(() => new ElectricScooter(Next(ElectricScooter.Prefix), "Borderline scooter", 8, 50, 0.35, 25));
        Add(() => new Bicycle(Next(Bicycle.Prefix), "Basic bicycle", 5, 70));
        Add(() => new Bicycle(Next(Bicycle.Prefix), "Comfortable bicycle", 6, 75));
        Add(() => new ElectricBicycle(Next(ElectricBicycle.Prefix), "Electric bicycle", 7, 85, 0.55));
    }

    private void AcceptThings()
    {
        printer.Title("2. Things on balance (no inspection needed)");

        Add(() => new Helmet(Next(Helmet.Prefix), "Adult helmet"));
        Add(() => new Helmet(Next(Helmet.Prefix), "Child helmet"));
        Add(() => new DockStation(Next(DockStation.Prefix), "Dock station", 1.2));
        Add(() => new ChargingCabinet(Next(ChargingCabinet.Prefix), "Charging cabinet", 1.4));
    }

    private void ShowErrors()
    {
        printer.Title("3. Error situations");

        var number = Next(Helmet.Prefix);
        Add(() => new Helmet(number, "New helmet"));

        printer.Info("The same number a second time:");
        Add(() => new Helmet(number, "Helmet with the same number"));

        printer.Info("Ease of use 11:");
        Add(() => new Bicycle(Next(Bicycle.Prefix), "Bicycle with ease of use 11", 11, 70));

        printer.Info("Number with a wrong prefix:");
        Add(() => new Bicycle("HE-100", "Bicycle with a wrong prefix", 5, 70));
    }

    private void ShowReports()
    {
        printer.Title("4. Reports");

        printer.Info("Items on balance:");
        printer.Items(reports.GetInventoryList());

        printer.Info(string.Empty);
        printer.Info($"Total items: {reports.GetTotalCount()}");
        foreach (var (kind, count) in reports.CountByKind())
            printer.Info($"  {kind}: {count}");

        printer.Info(string.Empty);
        printer.Info($"Total daily consumption: {printer.FormatKwh(reports.GetTotalDailyEnergyKwh())}");

        printer.Info(string.Empty);
        printer.Info("Suitable for novices:");
        printer.Items(reports.GetNoviceFriendlyItems());
    }

    private string Next(string prefix) => generator.Next(prefix, fleet.Items);

    private void Add(Func<IInventoryItem> create)
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