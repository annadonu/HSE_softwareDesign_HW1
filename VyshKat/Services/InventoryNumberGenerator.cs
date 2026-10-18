// выдает следующий инвентарный номер вида SC-003: максимальный номер с этим префиксом + 1.
using Vyshkat.Interfaces;

namespace Vyshkat.Services;

internal sealed class InventoryNumberGenerator
{
    public string Next(string prefix, IEnumerable<IInventoryItem> existing)
    {
        var start = prefix + "-";

        var max = existing
            .Select(i => i.InventoryNumber)
            .Where(n => n.StartsWith(start, StringComparison.OrdinalIgnoreCase))
            .Select(n => int.TryParse(n[start.Length..], out var value) && value > 0 ? value : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}-{max + 1:D3}";
    }
}