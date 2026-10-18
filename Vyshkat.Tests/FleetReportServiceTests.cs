// тесты отчетов: общий итог, подсчет по видам, энергия только от потребителей, список для новичков и сортировка.
using Vyshkat.Models;
using Vyshkat.Models.Things;
using Vyshkat.Models.Transports;
using Vyshkat.Services;
using Vyshkat.Tests;
using Xunit;

namespace Vyshkat.Tests;

public class FleetReportServiceTests
{
    private static (Fleet Fleet, FleetReportService Reports) Create()
    {
        var fleet = TestFactory.AcceptingFleet();
        return (fleet, new FleetReportService(fleet));
    }

    [Fact]
    public void GetTotalCount_EmptyFleet_ReturnsZero()
    {
        var (_, reports) = Create();

        Assert.Equal(0, reports.GetTotalCount());
    }

    [Fact]
    public void GetTotalCount_CountsTransportAndThings()
    {
        var (fleet, reports) = Create();
        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, 25));
        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Equal(3, reports.GetTotalCount());
    }

    [Fact]
    public void GetTotalCount_RejectedTransport_IsNotCounted()
    {
        var fleet = new Fleet(new ServiceCenter());
        var reports = new FleetReportService(fleet);
        fleet.Add(new Bicycle("BI-001", "Good", 6, 80));
        fleet.Add(new Bicycle("BI-002", "Worn", 6, 49));

        Assert.Equal(1, reports.GetTotalCount());
    }

    [Fact]
    public void GetTotalCount_ReflectsItemsAddedAfterServiceCreation()
    {
        var (fleet, reports) = Create();
        Assert.Equal(0, reports.GetTotalCount());

        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Equal(1, reports.GetTotalCount());
    }

    [Fact]
    public void CountByKind_EmptyFleet_ReturnsEmptyDictionary()
    {
        var (_, reports) = Create();

        Assert.Empty(reports.CountByKind());
    }

    [Fact]
    public void CountByKind_GroupsItemsByTypeName()
    {
        var (fleet, reports) = Create();
        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new Bicycle("BI-002", "Bicycle", 6, 80));
        fleet.Add(new Helmet("HE-001", "Helmet"));
        fleet.Add(new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, 25));

        var counts = reports.CountByKind();

        Assert.Equal(3, counts.Count);
        Assert.Equal(2, counts["Bicycle"]);
        Assert.Equal(1, counts["Helmet"]);
        Assert.Equal(1, counts["ElectricScooter"]);
    }

    [Fact]
    public void GetTotalDailyEnergyKwh_EmptyFleet_ReturnsZero()
    {
        var (_, reports) = Create();

        Assert.Equal(0, reports.GetTotalDailyEnergyKwh());
    }

    [Fact]
    public void GetTotalDailyEnergyKwh_OnlyNonConsumers_ReturnsZero()
    {
        var (fleet, reports) = Create();
        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Equal(0, reports.GetTotalDailyEnergyKwh());
    }

    [Fact]
    public void GetTotalDailyEnergyKwh_SumsTransportAndThingsConsumers()
    {
        var (fleet, reports) = Create();
        fleet.Add(new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, 25));
        fleet.Add(new ElectricBicycle("EB-001", "E-bike", 7, 85, 0.55));
        fleet.Add(new DockStation("DS-001", "Dock", 1.2));
        fleet.Add(new ChargingCabinet("CC-001", "Cabinet", 1.4));

        Assert.Equal(3.5, reports.GetTotalDailyEnergyKwh(), 6);
    }

    [Fact]
    public void GetTotalDailyEnergyKwh_BicycleAndHelmetDoNotChangeTotal()
    {
        var (fleet, reports) = Create();
        fleet.Add(new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, 25));
        var before = reports.GetTotalDailyEnergyKwh();

        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Equal(before, reports.GetTotalDailyEnergyKwh());
    }

    [Fact]
    public void GetTotalDailyEnergyKwh_RejectedConsumer_IsNotCounted()
    {
        var fleet = new Fleet(new ServiceCenter());
        var reports = new FleetReportService(fleet);
        fleet.Add(new ElectricScooter("SC-001", "Good", 8, 80, 0.35, 25));
        fleet.Add(new ElectricScooter("SC-002", "Worn", 8, 40, 0.35, 25));

        Assert.Equal(0.35, reports.GetTotalDailyEnergyKwh(), 6);
    }

    [Fact]
    public void GetNoviceFriendlyItems_EmptyFleet_ReturnsEmptyList()
    {
        var (_, reports) = Create();

        Assert.Empty(reports.GetNoviceFriendlyItems());
    }

    [Fact]
    public void GetNoviceFriendlyItems_ReturnsOnlySuitableTransportSortedByNumber()
    {
        var (fleet, reports) = Create();
        fleet.Add(new ElectricScooter("SC-001", "City scooter", 8, 80, 0.35, 25));
        fleet.Add(new ElectricScooter("SC-002", "Fast scooter", 8, 90, 0.35, 30));
        fleet.Add(new Bicycle("BI-001", "Basic bicycle", 5, 70));
        fleet.Add(new Bicycle("BI-002", "Comfortable bicycle", 6, 75));
        fleet.Add(new ElectricBicycle("EB-001", "E-bike", 7, 85, 0.55));
        fleet.Add(new Helmet("HE-001", "Helmet"));
        fleet.Add(new ChargingCabinet("CC-001", "Cabinet", 1.4));

        var numbers = reports.GetNoviceFriendlyItems().Select(i => i.InventoryNumber);

        Assert.Equal(["BI-002", "EB-001", "SC-001"], numbers);
    }

    [Fact]
    public void GetNoviceFriendlyItems_ThingsAreNeverIncluded()
    {
        var (fleet, reports) = Create();
        fleet.Add(new Helmet("HE-001", "Helmet"));
        fleet.Add(new DockStation("DS-001", "Dock", 1.2));
        fleet.Add(new ChargingCabinet("CC-001", "Cabinet", 1.4));

        Assert.Empty(reports.GetNoviceFriendlyItems());
    }

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    public void GetNoviceFriendlyItems_EaseOfUseBoundary(int easeOfUse, bool expectedInList)
    {
        var (fleet, reports) = Create();
        fleet.Add(new Bicycle("BI-001", "Bicycle", easeOfUse, 80));

        Assert.Equal(expectedInList, reports.GetNoviceFriendlyItems().Count == 1);
    }

    [Fact]
    public void GetNoviceFriendlyItems_RejectedTransport_IsNotIncluded()
    {
        var fleet = new Fleet(new ServiceCenter());
        var reports = new FleetReportService(fleet);
        fleet.Add(new Bicycle("BI-001", "Worn but simple", 10, 49));

        Assert.Empty(reports.GetNoviceFriendlyItems());
    }

    [Fact]
    public void GetInventoryList_EmptyFleet_ReturnsEmptyList()
    {
        var (_, reports) = Create();

        Assert.Empty(reports.GetInventoryList());
    }

    [Fact]
    public void GetInventoryList_ReturnsAllItemsSortedByNumber()
    {
        var (fleet, reports) = Create();
        fleet.Add(new ElectricScooter("SC-002", "Second scooter", 8, 80, 0.35, 25));
        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new Helmet("HE-001", "Helmet"));
        fleet.Add(new ElectricScooter("SC-001", "First scooter", 8, 80, 0.35, 25));

        var numbers = reports.GetInventoryList().Select(i => i.InventoryNumber);

        Assert.Equal(["BI-001", "HE-001", "SC-001", "SC-002"], numbers);
    }

    [Fact]
    public void GetInventoryList_ItemsKeepNameAndNumber()
    {
        var (fleet, reports) = Create();
        fleet.Add(new Helmet("HE-001", "Adult helmet"));

        var item = Assert.Single(reports.GetInventoryList());

        Assert.Equal("HE-001", item.InventoryNumber);
        Assert.Equal("Adult helmet", item.Name);
    }
    
    [Fact]
    public void GetInventoryList_ReturnsSnapshotNotLiveView()
    {
        var (fleet, reports) = Create();

        fleet.Add(new Helmet("HE-001", "First helmet"));

        var snapshot = reports.GetInventoryList();

        fleet.Add(new Helmet("HE-002", "Second helmet"));

        Assert.Equal(
            ["HE-001"],
            snapshot.Select(item => item.InventoryNumber));

        Assert.Equal(
            ["HE-001", "HE-002"],
            reports.GetInventoryList()
                .Select(item => item.InventoryNumber));
    }
}

