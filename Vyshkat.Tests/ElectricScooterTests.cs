// тесты электросамоката: правило скорости для новичков, расход энергии и диапазоны.
using Vyshkat.Interfaces;
using Vyshkat.Models.Transports;
using Xunit;

namespace Vyshkat.Tests;

public class ElectricScooterTests
{
    [Theory]
    [InlineData(8, 25, true)]
    [InlineData(8, 26, false)]
    [InlineData(8, 30, false)]
    [InlineData(6, 25, true)]
    [InlineData(10, 1, true)]
    [InlineData(5, 20, false)]
    [InlineData(5, 26, false)]
    public void IsNoviceFriendly_DependsOnEaseOfUseAndSpeed(int easeOfUse, int speed, bool expected)
    {
        var scooter = new ElectricScooter("SC-001", "Scooter", easeOfUse, 80, 0.35, speed);

        Assert.Equal(expected, scooter.IsNoviceFriendly);
    }

    [Fact]
    public void IsNoviceFriendly_FastScooterWithHighEaseOfUse_StillNotForNovices()
    {
        var scooter = new ElectricScooter("SC-001", "Fast scooter", 8, 90, 0.35, 30);
        Transport asTransport = scooter;
        IEaseOfUseRated asRole = scooter;

        Assert.True(scooter.EaseOfUse >= Transport.NoviceEaseOfUseLimit);
        Assert.False(asTransport.IsNoviceFriendly);
        Assert.False(asRole.IsNoviceFriendly);
    }

    [Fact]
    public void Ctor_ValidArguments_PropertiesHaveExpectedValues()
    {
        var scooter = new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, 25);

        Assert.Equal("SC-001", scooter.InventoryNumber);
        Assert.Equal(0.35, scooter.DailyEnergyKwh);
        Assert.Equal(25, scooter.MaxSpeedKmh);
        Assert.IsAssignableFrom<IEnergyConsumer>(scooter);
    }

    [Fact]
    public void Ctor_ZeroEnergy_DoesNotThrow()
    {
        var scooter = new ElectricScooter("SC-001", "Scooter", 8, 80, 0, 25);

        Assert.Equal(0, scooter.DailyEnergyKwh);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Ctor_InvalidEnergy_Throws(double energy)
    {
        Assert.Throws<ArgumentException>(() => new ElectricScooter("SC-001", "Scooter", 8, 80, energy, 25));
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void Ctor_NonPositiveSpeed_Throws(int speed)
    {
        Assert.Throws<ArgumentException>(() => new ElectricScooter("SC-001", "Scooter", 8, 80, 0.35, speed));
    }

    [Fact]
    public void Ctor_WrongPrefix_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ElectricScooter("BI-001", "Scooter", 8, 80, 0.35, 25));
    }

    [Fact]
    public void Ctor_InvalidBaseArguments_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ElectricScooter("SC-001", "Scooter", 11, 80, 0.35, 25));
        Assert.Throws<ArgumentException>(() => new ElectricScooter("SC-001", "Scooter", 8, 101, 0.35, 25));
    }
}