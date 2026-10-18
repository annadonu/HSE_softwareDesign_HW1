// контракт техосмотра: возвращает null, если объект принят, иначе причину отказа.
namespace Vyshkat.Interfaces;

public interface IServiceCenter
{
    string? Inspect(IInspectable item);
}