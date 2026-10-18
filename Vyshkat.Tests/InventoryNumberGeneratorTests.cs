// тесты генератора номеров: следующий номер по максимуму, пропуски, чужие префиксы и мусорные номера.
using Vyshkat.Interfaces;
using Vyshkat.Models;
using Vyshkat.Models.Transports;
using Vyshkat.Services;
using Vyshkat.Tests;
using Xunit;

namespace Vyshkat.Tests;

public class InventoryNumberGeneratorTests
{
    private readonly InventoryNumberGenerator _generator = new();

    private string Next(string prefix, params string[] existingNumbers)
    {
        var items = existingNumbers.Select(TestFactory.ItemWithNumber).ToList();
        return _generator.Next(prefix, items);
    }

    [Fact]
    public void Next_EmptyList_ReturnsFirstNumber()
    {
        Assert.Equal("SC-001", Next("SC"));
    }

    [Fact]
    public void Next_SequentialNumbers_ReturnsNextOne()
    {
        Assert.Equal("SC-003", Next("SC", "SC-001", "SC-002"));
    }

    [Fact]
    public void Next_GapInNumbers_UsesMaximumNotCount()
    {
        Assert.Equal("SC-006", Next("SC", "SC-001", "SC-005"));
    }

    [Fact]
    public void Next_UnorderedList_UsesMaximum()
    {
        Assert.Equal("SC-006", Next("SC", "SC-005", "SC-002"));
    }

    [Fact]
    public void Next_OtherPrefixOnly_StartsFromOne()
    {
        Assert.Equal("SC-001", Next("SC", "BI-007"));
    }

    [Fact]
    public void Next_PrefixesAreIndependent()
    {
        Assert.Equal("BI-004", Next("BI", "BI-003", "SC-010"));
    }

    [Fact]
    public void Next_PrefixThatOnlyStartsWithSameLetters_IsIgnored()
    {
        Assert.Equal("SC-001", Next("SC", "SCX-009"));
    }

    [Theory]
    [InlineData("SC-abc")]
    [InlineData("SC-")]
    [InlineData("SC--5")]
    [InlineData("SC-0")]
    public void Next_InvalidSuffix_IsIgnored(string number)
    {
        Assert.Equal("SC-001", Next("SC", number));
    }

    [Fact]
    public void Next_LowerCasePrefixInExistingNumber_IsCounted()
    {
        Assert.Equal("SC-005", Next("SC", "sc-004"));
    }

    [Fact]
    public void Next_MoreThanThreeDigits_KeepsAllDigits()
    {
        Assert.Equal("SC-1000", Next("SC", "SC-999"));
    }

    [Fact]
    public void Next_CalledTwiceWithoutAdding_ReturnsSameNumber()
    {
        var items = new List<IInventoryItem> { TestFactory.ItemWithNumber("SC-001") };

        var first = _generator.Next("SC", items);
        var second = _generator.Next("SC", items);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Next_AfterRejectedTransport_ReusesNumber()
    {
        var fleet = new Fleet(new ServiceCenter());
        fleet.Add(new ElectricScooter("SC-001", "Good scooter", 8, 80, 0.35, 25));
        var rejectedNumber = _generator.Next(ElectricScooter.Prefix, fleet.Items);
        var rejection = fleet.Add(new ElectricScooter(rejectedNumber, "Worn scooter", 8, 40, 0.35, 25));

        var nextNumber = _generator.Next(ElectricScooter.Prefix, fleet.Items);

        Assert.NotNull(rejection);
        Assert.Equal("SC-002", rejectedNumber);
        Assert.Equal("SC-002", nextNumber);
    }
}