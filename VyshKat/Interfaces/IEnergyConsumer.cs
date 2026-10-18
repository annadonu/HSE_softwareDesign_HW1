// потребляет энергию: суточный расход в кВт·ч.
namespace Vyshkat.Interfaces;

public interface IEnergyConsumer
{
    double DailyEnergyKwh { get; }
}