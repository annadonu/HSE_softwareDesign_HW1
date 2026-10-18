// электросамокат: расход энергии и скорость; новичкам не выдается, если скорость выше 25 км/ч.
using Vyshkat.Interfaces;

namespace Vyshkat.Models.Transports;

public sealed class ElectricScooter : Transport, IEnergyConsumer
{
    public const string Prefix = "SC";
    public const int NoviceMaxSpeedKmh = 25;

    public ElectricScooter(string inventoryNumber, string name, int easeOfUse, int conditionScore, double dailyEnergyKwh, int maxSpeedKmh)
        : base(Prefix, inventoryNumber, name, easeOfUse, conditionScore)
    {
        DailyEnergyKwh = !double.IsFinite(dailyEnergyKwh) || dailyEnergyKwh < 0
            ? throw new ArgumentException("DailyEnergyKwh must be a finite number not less than 0")
            : dailyEnergyKwh;
        MaxSpeedKmh = maxSpeedKmh <= 0
            ? throw new ArgumentException("MaxSpeedKmh must be greater than 0")
            : maxSpeedKmh;
    }

    public double DailyEnergyKwh { get; }
    public int MaxSpeedKmh { get; }

    public override bool IsNoviceFriendly => base.IsNoviceFriendly && MaxSpeedKmh <= NoviceMaxSpeedKmh;
}