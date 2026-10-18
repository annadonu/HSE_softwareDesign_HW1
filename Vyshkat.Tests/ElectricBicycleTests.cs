// тесты электровелосипеда: расход энергии, наследование правила для новичков и префикс номера.
using Vyshkat.Interfaces;
using Vyshkat.Models.Transports;
using Xunit;

namespace Vyshkat.Tests;

public class ElectricBicycleTests
{
    [Fact]
    public void Ctor_ValidArguments_PropertiesHaveExpectedValues()
    {
        var bicycle = new ElectricBicycle("EB-001", "E-bike", 7, 85, 0.55);

        Assert.Equal("EB-001", bicycle.InventoryNumber);
        Assert.Equal(0.55, bicycle.DailyEnergyKwh);
        Assert.IsAssignableFrom<IEnergyConsumer>(bicycle);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Ctor_InvalidEnergy_Throws(double energy)
    {
        Assert.Throws<ArgumentException>(() => new ElectricBicycle("EB-001", "E-bike", 7, 85, energy));
    }

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    public void IsNoviceFriendly_UsesBaseRule(int easeOfUse, bool expected)
    {
        var bicycle = new ElectricBicycle("EB-001", "E-bike", easeOfUse, 85, 0.55);

        Assert.Equal(expected, bicycle.IsNoviceFriendly);
    }

    [Fact]
    public void Ctor_WrongPrefix_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ElectricBicycle("BI-001", "E-bike", 7, 85, 0.55));
    }
}