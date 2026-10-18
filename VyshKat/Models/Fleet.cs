// хранилище принятого имущества: проверяет дубль номера, вызывает техосмотр и добавляет объект.
using Vyshkat.Interfaces;

namespace Vyshkat.Models;

public sealed class Fleet
{
    private readonly IServiceCenter _serviceCenter;
    private readonly List<IInventoryItem> _items = [];

    public Fleet(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter;
        Items = _items.AsReadOnly();
    }

    public IReadOnlyCollection<IInventoryItem> Items { get; }

    public string? Add(IInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var number = item.InventoryNumber.Trim();
        if (_items.Any(i => string.Equals(i.InventoryNumber.Trim(), number, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"Inventory number {number} is already in use");

        if (item is IInspectable inspectable)
        {
            var rejection = _serviceCenter.Inspect(inspectable);
            if (rejection is not null)
                return rejection;
        }

        _items.Add(item);
        return null;
    }
}