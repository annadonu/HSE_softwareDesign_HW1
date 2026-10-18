// есть оценка простоты для новичка: простота 1–10 и готовый признак пригодности.
namespace Vyshkat.Interfaces;

public interface IEaseOfUseRated
{
    int EaseOfUse { get; }
    bool IsNoviceFriendly { get; }
}