// тесты техосмотра: граница 49 и 50, крайние значения оценки и текст причины отказа.
using NSubstitute;
using Vyshkat.Interfaces;
using Vyshkat.Models.Transports;
using Vyshkat.Services;
using Xunit;

namespace Vyshkat.Tests;

public class ServiceCenterTests
{
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(49)]
    public void Inspect_ScoreBelowLimit_ReturnsWearReason(int score)
    {
        var serviceCenter = new ServiceCenter();
        var item = Substitute.For<IInspectable>();
        item.ConditionScore.Returns(score);

        var result = serviceCenter.Inspect(item);

        Assert.Equal(ServiceCenter.WearReason, result);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(51)]
    [InlineData(99)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Inspect_ScoreAtOrAboveLimit_ReturnsNull(int score)
    {
        var serviceCenter = new ServiceCenter();
        var item = Substitute.For<IInspectable>();
        item.ConditionScore.Returns(score);

        var result = serviceCenter.Inspect(item);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(49, true)]
    [InlineData(50, false)]
    public void Inspect_RealTransport_RejectsOnlyBelowLimit(int score, bool rejected)
    {
        var serviceCenter = new ServiceCenter();
        var bicycle = new Bicycle("BI-001", "Bicycle", 6, score);

        var result = serviceCenter.Inspect(bicycle);

        Assert.Equal(rejected, result is not null);
    }

    [Fact]
    public void Constants_HaveDocumentedValues()
    {
        Assert.Equal(50, ServiceCenter.MinConditionScore);
        Assert.Equal("Vehicle wear", ServiceCenter.WearReason);
    }
}