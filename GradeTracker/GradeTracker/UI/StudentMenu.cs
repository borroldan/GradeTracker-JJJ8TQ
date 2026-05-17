using GradeTracker.Data;
using GradeTracker.Models;

namespace GradeTracker.UI;

internal sealed class StudentMenu(Student student, IDataStoreRepository repository)
{
    private sealed record GradeDetail(
        string SubjectName,
        string AssignmentName,
        AssignmentType AssignmentType,
        double Score,
        double MaxScore,
        string Note,
        DateTime GradedAt)
    {
        public double Percentage => MaxScore > 0 ? Score / MaxScore * 100 : 0;
    }

    public void Run()
    {
        while (true)
        {
            MenuHelper.PrintHeader($"Student Menu — {student.Name}");
            Console.WriteLine("  1. View my grades");
            Console.WriteLine("  2. Exit");
            Console.WriteLine();

            switch (MenuHelper.PromptChoice(1, 2))
            {
                case 1: ViewMyGrades(); break;
                case 2: return;
            }
        }
    }

    private void ViewMyGrades()
    {
        MenuHelper.PrintHeader("My Grades");
        Console.Write("  Enter folder path to scan: ");
        string? folderPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            MenuHelper.PrintError("Folder does not exist.");
            MenuHelper.WaitForInput();
            return;
        }

        IEnumerable<string> files = repository.FindFilesContainingStudent(folderPath, student.Id);
        List<GradeDetail> allGrades = [];

        foreach (string file in files)
        {
            try
            {
                DataStore store = repository.Load(file);

                foreach (GradeEntry entry in store.GradeEntries.Where(e => e.StudentId == student.Id))
                {
                    Assignment? assignment = store.Assignments.FirstOrDefault(a => a.Id == entry.AssignmentId);
                    if (assignment is null) continue;

                    string subjectName = store.Subjects
                        .FirstOrDefault(s => s.Id == assignment.SubjectId)?.Name ?? "Unknown";

                    allGrades.Add(new GradeDetail(
                        subjectName,
                        assignment.Name,
                        assignment.Type,
                        entry.Score,
                        assignment.MaxScore,
                        entry.Note,
                        entry.GradedAt));
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"  Could not load: {Path.GetFileName(file)}");
            }
        }

        if (allGrades.Count == 0)
        {
            Console.WriteLine("  No grades found.");
            MenuHelper.WaitForInput();
            return;
        }

        Console.WriteLine("  --- All Grades ---");
        Console.WriteLine();

        foreach (GradeDetail grade in allGrades.OrderByDescending(g => g.GradedAt))
        {
            Console.WriteLine($"  Subject:    {grade.SubjectName}");
            Console.WriteLine($"  Assignment: {grade.AssignmentName} ({grade.AssignmentType})");
            Console.WriteLine($"  Score:      {grade.Score} / {grade.MaxScore} ({grade.Percentage:F1}%)");
            Console.WriteLine($"  Note:       {grade.Note}");
            Console.WriteLine($"  Graded:     {grade.GradedAt:g}");
            Console.WriteLine();
        }

        PrintSummary(allGrades);
        MenuHelper.WaitForInput();
    }

    private static void PrintSummary(List<GradeDetail> grades)
    {
        Console.WriteLine(new string('=', 45));
        Console.WriteLine("  Summary");
        Console.WriteLine(new string('=', 45));
        Console.WriteLine();

        double averagePercentage = grades.Average(g => g.Percentage);
        double bestPercentage = grades.Max(g => g.Percentage);
        double worstPercentage = grades.Min(g => g.Percentage);
        GradeDetail firstRecorded = grades.OrderBy(g => g.GradedAt).First();

        Console.WriteLine($"  Average:     {averagePercentage:F1}%");
        Console.WriteLine($"  Best score:  {bestPercentage:F1}%");
        Console.WriteLine($"  Worst score: {worstPercentage:F1}%");
        Console.WriteLine($"  First grade: {firstRecorded.AssignmentName} on {firstRecorded.GradedAt:d}");
        Console.WriteLine();

        Console.WriteLine("  --- Grouped by Subject ---");
        Console.WriteLine();

        foreach (var group in grades.GroupBy(g => g.SubjectName))
        {
            Console.WriteLine($"  [{group.Key}]");
            foreach (GradeDetail grade in group)
                Console.WriteLine($"    {grade.AssignmentName}: {grade.Score}/{grade.MaxScore} ({grade.Percentage:F1}%)");
            Console.WriteLine();
        }
    }
}
