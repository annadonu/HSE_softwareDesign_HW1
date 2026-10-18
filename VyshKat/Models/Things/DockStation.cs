// док-станция: вещь с суточным расходом энергии.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Things;

public sealed class DockStation : Thing, IEnergyConsumer
{
    public const string Prefix = "DS";

    public DockStation(string inventoryNumber, string name, double dailyEnergyKwh)
        : base(Prefix, inventoryNumber, name)
    {
        DailyEnergyKwh = !double.IsFinite(dailyEnergyKwh) || dailyEnergyKwh < 0
            ? throw new ArgumentException("DailyEnergyKwh must be a finite number not less than 0")
            : dailyEnergyKwh;
    }

    public double DailyEnergyKwh { get; }
}