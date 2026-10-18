// весь ввод с консоли: повторяет запрос, пока пользователь не введет корректное значение.
using System.Globalization;

namespace Vyshkat.Demo;

internal sealed class ConsoleReader
{
    public string ReadText(string prompt)
    {
        while (true)
        {
            var line = ReadLine(prompt).Trim();
            if (line.Length > 0)
                return line;

            Console.WriteLine("The value cannot be empty.");
        }
    }

    public int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            if (int.TryParse(ReadLine(prompt), out var value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Enter an integer from {min} to {max}.");
        }
    }

    public double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            var line = ReadLine(prompt).Replace(',', '.');
            if (double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                && value >= min && value <= max)
                return value;

            Console.WriteLine($"Enter a number from {min} to {max}.");
        }
    }

    private static string ReadLine(string prompt)
    {
        Console.Write($"{prompt}: ");
        return Console.ReadLine() ?? throw new OperationCanceledException();
    }
}