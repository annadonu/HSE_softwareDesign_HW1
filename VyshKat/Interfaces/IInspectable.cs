// требует техосмотра: оценка состояния от 0 до 100, чем выше, тем лучше.
namespace Vyshkat.Interfaces;

public interface IInspectable
{
    int ConditionScore { get; }
}