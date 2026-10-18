// общие вспомогательные методы для тестов: заглушки техосмотра и объекты с заданным номером.
using NSubstitute;
using Vyshkat.Interfaces;
using Vyshkat.Models;

namespace Vyshkat.Tests;

internal static class TestFactory
{
    public static IServiceCenter AcceptingServiceCenter()
    {
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Inspect(Arg.Any<IInspectable>()).Returns((string?)null);
        return serviceCenter;
    }

    public static IServiceCenter RejectingServiceCenter(string reason = "Vehicle wear")
    {
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Inspect(Arg.Any<IInspectable>()).Returns(reason);
        return serviceCenter;
    }

    public static Fleet AcceptingFleet() => new(AcceptingServiceCenter());

    public static IInventoryItem ItemWithNumber(string number)
    {
        var item = Substitute.For<IInventoryItem>();
        item.InventoryNumber.Returns(number);
        item.Name.Returns("Test item");
        return item;
    }
}