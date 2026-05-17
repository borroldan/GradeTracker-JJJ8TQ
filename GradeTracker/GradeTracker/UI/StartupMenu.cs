using GradeTracker.Data;
using GradeTracker.Models;

namespace GradeTracker.UI;

internal sealed class StartupMenu(DataStore baseStore)
{
    public User Run()
    {
        MenuHelper.PrintHeader("Welcome to GradeTracker");
        Console.WriteLine("  Select a user to log in:");
        Console.WriteLine();

        for (int i = 0; i < baseStore.Users.Count; i++)
        {
            User user = baseStore.Users[i];
            Console.WriteLine($"  {i + 1}. {user.Name} ({user.Role})");
        }

        Console.WriteLine();
        int choice = MenuHelper.PromptChoice(1, baseStore.Users.Count);
        return baseStore.Users[choice - 1];
    }
}
