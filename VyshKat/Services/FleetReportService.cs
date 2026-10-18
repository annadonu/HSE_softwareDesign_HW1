// отчеты по парку: количество, энергопотребление, список для новичков, список названий и номеров.
using Vyshkat.Interfaces;
using Vyshkat.Models;

namespace Vyshkat.Services;

public sealed class FleetReportService(Fleet fleet)
{
    public int GetTotalCount()
    {
        return fleet.Items.Count;
    }

    public IReadOnlyDictionary<string, int> CountByKind()
    {
        return fleet.Items
            .GroupBy(i => i.GetType().Name)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public double GetTotalDailyEnergyKwh()
    {
        return fleet.Items.OfType<IEnergyConsumer>().Sum(c => c.DailyEnergyKwh);
    }

    public IReadOnlyList<IInventoryItem> GetNoviceFriendlyItems()
    {
        return fleet.Items
            .Where(i => i is IEaseOfUseRated { IsNoviceFriendly: true })
            .OrderBy(i => i.InventoryNumber)
            .ToList();
    }

    public IReadOnlyList<IInventoryItem> GetInventoryList()
    {
        return fleet.Items.OrderBy(i => i.InventoryNumber).ToList();
    }
}