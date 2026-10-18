// базовый класс вещей на балансе: номер и название, техосмотр не требуется.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Things;

public abstract class Thing : IInventoryItem
{
    protected Thing(string prefix, string inventoryNumber, string name)
    {
        InventoryNumber = string.IsNullOrWhiteSpace(inventoryNumber) || !inventoryNumber.StartsWith(prefix + "-", StringComparison.Ordinal)
            ? throw new ArgumentException($"InventoryNumber must start with {prefix}-")
            : inventoryNumber;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name cannot be null or empty")
            : name;
    }

    public string InventoryNumber { get; }
    public string Name { get; }
}