// тесты парка: подмена техосмотра, отказ, вещи без осмотра, дубли номеров и защита коллекции.
using NSubstitute;
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Models.Things;
using Vyshkat.Models.Transports;
using Vyshkat.Services;
using Vyshkat.Tests;
using Xunit;

namespace Vyshkat.Tests;

public class FleetTests
{
    [Fact]
    public void Items_NewFleet_IsEmpty()
    {
        var fleet = TestFactory.AcceptingFleet();

        Assert.Empty(fleet.Items);
    }

    [Fact]
    public void Add_InspectionPassed_AddsItemAndReturnsNull()
    {
        var fleet = new Fleet(TestFactory.AcceptingServiceCenter());
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, 80);

        var result = fleet.Add(bicycle);

        Assert.Null(result);
        Assert.Same(bicycle, Assert.Single(fleet.Items));
    }

    [Fact]
    public void Add_InspectionRejected_ReturnsReasonAndDoesNotAddItem()
    {
        var fleet = new Fleet(TestFactory.RejectingServiceCenter("Vehicle wear"));
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, 10);

        var result = fleet.Add(bicycle);

        Assert.Equal("Vehicle wear", result);
        Assert.Empty(fleet.Items);
    }

    [Fact]
    public void Add_Transport_PassesSameObjectToServiceCenterOnce()
    {
        var serviceCenter = TestFactory.AcceptingServiceCenter();
        var fleet = new Fleet(serviceCenter);
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, 80);

        fleet.Add(bicycle);

        serviceCenter.Received(1).Inspect(bicycle);
    }

    [Fact]
    public void Add_Thing_DoesNotCallServiceCenterEvenIfItRejectsEverything()
    {
        var serviceCenter = TestFactory.RejectingServiceCenter();
        var fleet = new Fleet(serviceCenter);

        var result = fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Null(result);
        Assert.Single(fleet.Items);
        serviceCenter.DidNotReceive().Inspect(Arg.Any<IInspectable>());
    }

    [Fact]
    public void Add_RejectedTransport_DoesNotReserveItsNumber()
    {
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Inspect(Arg.Any<IInspectable>()).Returns("Vehicle wear", (string?)null);
        var fleet = new Fleet(serviceCenter);
        var accepted = new Bicycle("BI-001", "Second try", 6, 80);

        var firstResult = fleet.Add(new Bicycle("BI-001", "First try", 6, 10));
        var secondResult = fleet.Add(accepted);

        Assert.Equal("Vehicle wear", firstResult);
        Assert.Null(secondResult);
        Assert.Same(accepted, Assert.Single(fleet.Items));
    }

    [Fact]
    public void Add_DuplicateNumber_ThrowsAndKeepsFirstItem()
    {
        var fleet = TestFactory.AcceptingFleet();
        var first = new Bicycle("BI-001", "First", 6, 80);
        fleet.Add(first);

        Assert.Throws<ArgumentException>(() => fleet.Add(new Bicycle("BI-001", "Second", 6, 80)));

        Assert.Same(first, Assert.Single(fleet.Items));
    }

    [Fact]
    public void Add_DuplicateNumberOfWornTransport_ThrowsAndSkipsInspection()
    {
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Inspect(Arg.Any<IInspectable>()).Returns((string?)null, "Vehicle wear");
        var fleet = new Fleet(serviceCenter);
        fleet.Add(new Bicycle("BI-001", "First", 6, 80));

        Assert.Throws<ArgumentException>(() => fleet.Add(new Bicycle("BI-001", "Worn", 6, 10)));

        serviceCenter.Received(1).Inspect(Arg.Any<IInspectable>());
    }

    [Theory]
    [InlineData("he-001")]
    [InlineData(" HE-001")]
    [InlineData("HE-001 ")]
    [InlineData("  he-001  ")]
    public void Add_SameNumberWithDifferentCaseOrSpaces_Throws(string duplicate)
    {
        var fleet = TestFactory.AcceptingFleet();
        fleet.Add(TestFactory.ItemWithNumber("HE-001"));

        Assert.Throws<ArgumentException>(() => fleet.Add(TestFactory.ItemWithNumber(duplicate)));
    }

    [Fact]
    public void Add_DifferentNumbers_AddsAllItems()
    {
        var fleet = TestFactory.AcceptingFleet();

        fleet.Add(new Bicycle("BI-001", "Bicycle", 6, 80));
        fleet.Add(new Bicycle("BI-002", "Bicycle", 6, 80));
        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Equal(3, fleet.Items.Count);
    }

    [Fact]
    public void Add_Null_ThrowsArgumentNullException()
    {
        var fleet = TestFactory.AcceptingFleet();

        Assert.Throws<ArgumentNullException>(() => fleet.Add(null!));
    }

    [Theory]
    [InlineData(0, "Vehicle wear")]
    [InlineData(49, "Vehicle wear")]
    [InlineData(50, null)]
    [InlineData(100, null)]
    public void Add_WithRealServiceCenter_UsesInspectionLimit(int conditionScore, string? expected)
    {
        var fleet = new Fleet(new ServiceCenter());

        var result = fleet.Add(new Bicycle("BI-001", "Bicycle", 6, conditionScore));

        Assert.Equal(expected, result);
        Assert.Equal(expected is null ? 1 : 0, fleet.Items.Count);
    }

    [Fact]
    public void Items_IsReadOnlyViewThatReflectsLaterChanges()
    {
        var fleet = TestFactory.AcceptingFleet();
        var items = fleet.Items;

        fleet.Add(new Helmet("HE-001", "Helmet"));

        Assert.Single(items);
        Assert.IsNotType<List<IInventoryItem>>(items);
        var list = Assert.IsAssignableFrom<IList<IInventoryItem>>(items);
        Assert.True(list.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => list.Add(new Helmet("HE-002", "Helmet")));
    }
}