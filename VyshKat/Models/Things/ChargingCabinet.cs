// зарядный шкаф: вещь с суточным расходом энергии.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Things;

public sealed class ChargingCabinet : Thing, IEnergyConsumer
{
    public const string Prefix = "CC";

    public ChargingCabinet(string inventoryNumber, string name, double dailyEnergyKwh)
        : base(Prefix, inventoryNumber, name)
    {
        DailyEnergyKwh = !double.IsFinite(dailyEnergyKwh) || dailyEnergyKwh < 0
            ? throw new ArgumentException("DailyEnergyKwh must be a finite number not less than 0")
            : dailyEnergyKwh;
    }

    public double DailyEnergyKwh { get; }
}