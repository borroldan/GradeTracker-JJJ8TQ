namespace GradeTracker.UI;

internal static class MenuHelper
{
    public static int PromptChoice(int min, int max)
    {
        while (true)
        {
            Console.Write("> ");
            try
            {
                string? input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
                    return choice;
            }
            catch (InvalidOperationException)
            {
            }

            Console.WriteLine($"  Please enter a number between {min} and {max}.");
        }
    }

    public static void PrintHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 45));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 45));
        Console.WriteLine();
    }

    public static void PrintError(string message) =>
        Console.WriteLine($"  [ERROR] {message}");

    public static void WaitForInput()
    {
        Console.WriteLine();
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey(true);
    }
}
