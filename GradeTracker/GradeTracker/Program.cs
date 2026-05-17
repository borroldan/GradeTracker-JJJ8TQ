using GradeTracker.Data;
using GradeTracker.Models;
using GradeTracker.UI;

const string BaseFilePath = "data.json";

try
{
    DataSeeder seeder = new();
    seeder.Seed(BaseFilePath);

    DataStoreRepository repository = new();

    while (true)
    {
        DataStore baseStore = repository.Load(BaseFilePath);
        StartupMenu startupMenu = new(baseStore);
        User selectedUser = startupMenu.Run();

        switch (selectedUser.Role)
        {
            case UserRole.Admin:
                new AdminMenu(baseStore, repository, BaseFilePath).Run();
                break;

            case UserRole.Teacher:
                new TeacherMenu((Teacher)selectedUser, baseStore, repository).Run();
                break;

            case UserRole.Student:
                new StudentMenu((Student)selectedUser, repository).Run();
                break;
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}
