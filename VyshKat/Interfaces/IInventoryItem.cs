// стоит на балансе: инвентарный номер и наименование.
namespace Vyshkat.Interfaces;

public interface IInventoryItem
{
    string InventoryNumber { get; }
    string Name { get; }
}