using GradeTracker.Models;

namespace GradeTracker.Data;

internal sealed class SessionPathCollector
{
    public SessionPathResult Collect()
    {
        string folderPath = PromptForValidFolder();
        string fileName = PromptForFileName();
        return new SessionPathResult { FolderPath = folderPath, FileName = fileName };
    }

    private static string PromptForValidFolder()
    {
        while (true)
        {
            Console.Write("Enter folder path: ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Folder path cannot be empty.");
                continue;
            }

            if (Directory.Exists(input))
                return input;

            Console.WriteLine($"Folder '{input}' does not exist. Please try again.");
        }
    }

    private static string PromptForFileName()
    {
        while (true)
        {
            Console.Write("Enter file name (without extension): ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("File name cannot be empty.");
                continue;
            }

            return input.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? input
                : $"{input}.json";
        }
    }
}
