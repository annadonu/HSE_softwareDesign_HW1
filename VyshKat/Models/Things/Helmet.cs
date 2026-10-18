// шлем: простая вещь без энергопотребления.
namespace Vyshkat.Models.Things;

public sealed class Helmet(string inventoryNumber, string name)
    : Thing(Prefix, inventoryNumber, name)
{
    public const string Prefix = "HE";
}