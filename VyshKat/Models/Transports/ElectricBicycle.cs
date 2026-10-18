// электровелосипед: транспорт с суточным расходом энергии.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Transports;

public sealed class ElectricBicycle : Transport, IEnergyConsumer
{
    public const string Prefix = "EB";

    public ElectricBicycle(string inventoryNumber, string name, int easeOfUse, int conditionScore, double dailyEnergyKwh)
        : base(Prefix, inventoryNumber, name, easeOfUse, conditionScore)
    {
        DailyEnergyKwh = !double.IsFinite(dailyEnergyKwh) || dailyEnergyKwh < 0
            ? throw new ArgumentException("DailyEnergyKwh must be a finite number not less than 0")
            : dailyEnergyKwh;
    }

    public double DailyEnergyKwh { get; }
}