// реальный техосмотр: оценка состояния ниже 50 означает отказ.
using Vyshkat.Interfaces;

namespace Vyshkat.Services;

internal sealed class ServiceCenter : IServiceCenter
{
    public const int MinConditionScore = 50;
    public const string WearReason = "Vehicle wear";

    public string? Inspect(IInspectable item)
    {
        return item.ConditionScore < MinConditionScore ? WearReason : null;
    }
}