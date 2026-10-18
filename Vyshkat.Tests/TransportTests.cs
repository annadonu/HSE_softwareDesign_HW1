// тесты общих правил транспорта (через Bicycle): диапазоны, префикс номера и граница 5 и 6 для новичков.
using Vyshkat.Interfaces;
using Vyshkat.Models.Transports;
using Xunit;

namespace Vyshkat.Tests;

public class TransportTests
{
    [Fact]
    public void Ctor_ValidArguments_PropertiesHaveExpectedValues()
    {
        var bicycle = new Bicycle("BI-001", "City bicycle", 7, 80);

        Assert.Equal("BI-001", bicycle.InventoryNumber);
        Assert.Equal("City bicycle", bicycle.Name);
        Assert.Equal(7, bicycle.EaseOfUse);
        Assert.Equal(80, bicycle.ConditionScore);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Ctor_EaseOfUseOnBoundary_DoesNotThrow(int easeOfUse)
    {
        var bicycle = new Bicycle("BI-001", "Bicycle", easeOfUse, 80);

        Assert.Equal(easeOfUse, bicycle.EaseOfUse);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(int.MaxValue)]
    public void Ctor_EaseOfUseOutOfRange_Throws(int easeOfUse)
    {
        Assert.Throws<ArgumentException>(() => new Bicycle("BI-001", "Bicycle", easeOfUse, 80));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Ctor_ConditionScoreOnBoundary_DoesNotThrow(int score)
    {
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, score);

        Assert.Equal(score, bicycle.ConditionScore);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(int.MaxValue)]
    public void Ctor_ConditionScoreOutOfRange_Throws(int score)
    {
        Assert.Throws<ArgumentException>(() => new Bicycle("BI-001", "Bicycle", 6, score));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_EmptyName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => new Bicycle("BI-001", name!, 6, 80));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("HE-001")]
    [InlineData("bi-001")]
    [InlineData(" BI-001")]
    [InlineData("BI001")]
    [InlineData("EB-001")]
    public void Ctor_NumberWithoutOwnPrefix_Throws(string? number)
    {
        Assert.Throws<ArgumentException>(() => new Bicycle(number!, "Bicycle", 6, 80));
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(7, true)]
    [InlineData(10, true)]
    public void IsNoviceFriendly_Bicycle_DependsOnEaseOfUseLimit(int easeOfUse, bool expected)
    {
        var bicycle = new Bicycle("BI-001", "Bicycle", easeOfUse, 80);

        Assert.Equal(expected, bicycle.IsNoviceFriendly);
    }

    [Fact]
    public void IsNoviceFriendly_DoesNotDependOnConditionScore()
    {
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, 0);

        Assert.True(bicycle.IsNoviceFriendly);
    }

    [Fact]
    public void Bicycle_ImplementsRolesWithoutEnergy()
    {
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, 80);

        Assert.IsAssignableFrom<IInventoryItem>(bicycle);
        Assert.IsAssignableFrom<IInspectable>(bicycle);
        Assert.IsAssignableFrom<IEaseOfUseRated>(bicycle);
        IInventoryItem item = bicycle;
        Assert.False(item is IEnergyConsumer);
    }
}