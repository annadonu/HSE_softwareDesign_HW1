using Microsoft.Extensions.DependencyInjection;
using Vyshkat;
using Vyshkat.Demo;
using Xunit;

namespace Vyshkat.Tests;

public class ConsoleApplicationTests
{
    [Fact]
    public void Program_Main_ExercisesMenuAndDemonstration()
    {
        var inputLines = new[]
        {
            // Проверка некорректного пункта меню и пустого списка.
            "invalid",
            "8",
            "3",

            // Обычный велосипед: повтор ввода пустого имени,
            // неправильной простоты и оценки состояния.
            "1",
            "1",
            "",
            "City bike",
            "11",
            "6",
            "-1",
            "80",

            // Электровелосипед: неправильный и слишком большой расход.
            "1",
            "2",
            "Electric bicycle",
            "7",
            "85",
            "not-a-number",
            "1001",
            "0,55",

            // Самокат: неправильная скорость.
            "1",
            "3",
            "City scooter",
            "8",
            "80",
            "0.35",
            "0",
            "25",

            // Все три вида вещей.
            "2",
            "1",
            "Helmet",

            "2",
            "2",
            "Dock station",
            "1.2",

            "2",
            "3",
            "Charging cabinet",
            "1.4",

            // Основные отчёты и готовая демонстрация.
            "3",
            "4",
            "5",
            "6",
            "7",

            // Demonstration.Pause вызывается четыре раза.
            "",
            "",
            "",
            "",

            // Выход из приложения.
            "0"
        };

        var input = string.Join(
            Environment.NewLine,
            inputLines) + Environment.NewLine;

        var output = CaptureConsole(input, Program.Main);

        Assert.Contains("The list is empty.", output);
        Assert.Contains("[ACCEPTED]", output);
        Assert.Contains("[REJECTED]", output);
        Assert.Contains("[ERROR]", output);
        Assert.Contains("Total daily consumption", output);
        Assert.Contains("Suitable for novices", output);
        Assert.Contains("ElectricScooter", output);
    }

    [Fact]
    public void MainMenu_EndOfInput_StopsGracefully()
    {
        var services = new ServiceCollection();
        Program.ConfigureServices(services);

        using var provider = services.BuildServiceProvider();

        var menu = provider.GetRequiredService<MainMenu>();

        var output = CaptureConsole(
            string.Empty,
            menu.Run);

        Assert.Contains("Choose an option", output);
    }

    private static string CaptureConsole(
        string input,
        Action action)
    {
        var originalInput = Console.In;
        var originalOutput = Console.Out;

        using var reader = new StringReader(input);
        using var writer = new StringWriter();

        try
        {
            Console.SetIn(reader);
            Console.SetOut(writer);

            action();

            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}