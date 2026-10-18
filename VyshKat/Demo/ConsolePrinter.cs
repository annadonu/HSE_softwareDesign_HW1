// весь вывод в консоль: заголовки, цветные сообщения и таблица имущества.
using System.Globalization;
using Vyshkat.Interfaces;

namespace Vyshkat.Demo;

internal sealed class ConsolePrinter
{
    public void Title(string text)
    {
        Console.WriteLine();
        WriteColored($"=== {text} ===", ConsoleColor.Cyan);
    }

    public void Info(string text) => Console.WriteLine(text);

    public void Success(string text) => WriteColored($"[ACCEPTED] {text}", ConsoleColor.Green);

    public void Rejected(string text) => WriteColored($"[REJECTED] {text}", ConsoleColor.Red);

    public void Error(string text) => WriteColored($"[ERROR] {text}", ConsoleColor.Yellow);

    public string FormatKwh(double value) => $"{value.ToString("F2", CultureInfo.InvariantCulture)} kWh";

    public void AddResult(IInventoryItem item, string? rejection)
    {
        if (rejection is null)
            Success($"{item.InventoryNumber} {item.Name}");
        else
            Rejected($"{item.Name}: {rejection}");
    }

    public void Items(IEnumerable<IInventoryItem> items)
    {
        var list = items.ToList();
        if (list.Count == 0)
        {
            Info("The list is empty.");
            return;
        }

        Console.WriteLine($"{"Number",-10}{"Kind",-18}Name");
        Console.WriteLine(new string('-', 50));
        foreach (var item in list)
            Console.WriteLine($"{item.InventoryNumber,-10}{item.GetType().Name,-18}{item.Name}");
    }

    public void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }

    private static void WriteColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}