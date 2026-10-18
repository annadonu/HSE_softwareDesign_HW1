// тесты вещей: проверка номера и названия, расход энергии у шкафа и док-станции, отсутствие лишних ролей.
using Vyshkat.Interfaces;
using Vyshkat.Models.Things;
using Xunit;

namespace Vyshkat.Tests;

public class ThingTests
{
    [Fact]
    public void Helmet_ValidArguments_PropertiesHaveExpectedValues()
    {
        var helmet = new Helmet("HE-001", "Helmet");

        Assert.Equal("HE-001", helmet.InventoryNumber);
        Assert.Equal("Helmet", helmet.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("SC-001")]
    [InlineData("he-001")]
    [InlineData(" HE-001")]
    public void Helmet_InvalidNumber_Throws(string? number)
    {
        Assert.Throws<ArgumentException>(() => new Helmet(number!, "Helmet"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Helmet_EmptyName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => new Helmet("HE-001", name!));
    }

    [Fact]
    public void Helmet_HasOnlyInventoryRole()
    {
        IInventoryItem item = new Helmet("HE-001", "Helmet");

        Assert.False(item is IEnergyConsumer);
        Assert.False(item is IInspectable);
        Assert.False(item is IEaseOfUseRated);
    }

    [Fact]
    public void DockStation_ValidArguments_PropertiesHaveExpectedValues()
    {
        var station = new DockStation("DS-001", "Dock", 1.2);

        Assert.Equal("DS-001", station.InventoryNumber);
        Assert.Equal(1.2, station.DailyEnergyKwh);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void DockStation_InvalidEnergy_Throws(double energy)
    {
        Assert.Throws<ArgumentException>(() => new DockStation("DS-001", "Dock", energy));
    }

    [Fact]
    public void DockStation_WrongPrefix_Throws()
    {
        Assert.Throws<ArgumentException>(() => new DockStation("CC-001", "Dock", 1.2));
    }

    [Fact]
    public void DockStation_IsEnergyConsumerWithoutInspectionOrEaseOfUse()
    {
        IInventoryItem item = new DockStation("DS-001", "Dock", 1.2);

        Assert.True(item is IEnergyConsumer);
        Assert.False(item is IInspectable);
        Assert.False(item is IEaseOfUseRated);
    }

    [Fact]
    public void ChargingCabinet_ValidArguments_PropertiesHaveExpectedValues()
    {
        var cabinet = new ChargingCabinet("CC-001", "Cabinet", 1.4);

        Assert.Equal("CC-001", cabinet.InventoryNumber);
        Assert.Equal(1.4, cabinet.DailyEnergyKwh);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void ChargingCabinet_InvalidEnergy_Throws(double energy)
    {
        Assert.Throws<ArgumentException>(() => new ChargingCabinet("CC-001", "Cabinet", energy));
    }

    [Fact]
    public void ChargingCabinet_WrongPrefix_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ChargingCabinet("DS-001", "Cabinet", 1.4));
    }

    [Fact]
    public void ChargingCabinet_IsEnergyConsumerWithoutInspectionOrEaseOfUse()
    {
        IInventoryItem item = new ChargingCabinet("CC-001", "Cabinet", 1.4);

        Assert.True(item is IEnergyConsumer);
        Assert.False(item is IInspectable);
        Assert.False(item is IEaseOfUseRated);
    }
}