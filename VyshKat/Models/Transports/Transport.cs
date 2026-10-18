// базовый класс транспорта: номер, название, простота, оценка состояния и правило для новичков.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Transports;

public abstract class Transport : IInventoryItem, IInspectable, IEaseOfUseRated
{
    public const int NoviceEaseOfUseLimit = 6;

    protected Transport(string prefix, string inventoryNumber, string name, int easeOfUse, int conditionScore)
    {
        InventoryNumber = string.IsNullOrWhiteSpace(inventoryNumber) || !inventoryNumber.StartsWith(prefix + "-", StringComparison.Ordinal)
            ? throw new ArgumentException($"InventoryNumber must start with {prefix}-")
            : inventoryNumber;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name cannot be null or empty")
            : name;
        EaseOfUse = easeOfUse is < 1 or > 10
            ? throw new ArgumentException("EaseOfUse must be between 1 and 10")
            : easeOfUse;
        ConditionScore = conditionScore is < 0 or > 100
            ? throw new ArgumentException("ConditionScore must be between 0 and 100")
            : conditionScore;
    }

    public string InventoryNumber { get; }
    public string Name { get; }
    public int EaseOfUse { get; }
    public int ConditionScore { get; }

    public virtual bool IsNoviceFriendly => EaseOfUse >= NoviceEaseOfUseLimit;
}