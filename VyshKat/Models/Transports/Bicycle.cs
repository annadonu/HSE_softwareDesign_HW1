// обычный велосипед: транспорт без энергопотребления.
namespace Vyshkat.Models.Transports;

public sealed class Bicycle(string inventoryNumber, string name, int easeOfUse, int conditionScore)
    : Transport(Prefix, inventoryNumber, name, easeOfUse, conditionScore)
{
    public const string Prefix = "BI";
}