using GradeTracker.Data;
using GradeTracker.Models;

namespace GradeTracker.UI;

internal sealed class TeacherMenu(Teacher teacher, DataStore baseStore, IDataStoreRepository repository)
{
    public void Run()
    {
        while (true)
        {
            MenuHelper.PrintHeader($"Teacher Menu — {teacher.Name}");
            Console.WriteLine("  1. Start grading session");
            Console.WriteLine("  2. View previous sessions");
            Console.WriteLine("  3. Exit");
            Console.WriteLine();

            switch (MenuHelper.PromptChoice(1, 3))
            {
                case 1: StartGradingSession(); break;
                case 2: ViewPreviousSessions(); break;
                case 3: return;
            }
        }
    }

    private void StartGradingSession()
    {
        MenuHelper.PrintHeader("Start Grading Session");

        List<Subject> teacherSubjects = baseStore.Subjects
            .Where(s => teacher.SubjectIds.Contains(s.Id))
            .ToList();

        if (teacherSubjects.Count == 0)
        {
            MenuHelper.PrintError("You have no subjects assigned.");
            MenuHelper.WaitForInput();
            return;
        }

        Console.WriteLine("  Select a subject:");
        for (int i = 0; i < teacherSubjects.Count; i++)
            Console.WriteLine($"  {i + 1}. {teacherSubjects[i].Name}");
        Console.WriteLine();

        Subject selectedSubject = teacherSubjects[MenuHelper.PromptChoice(1, teacherSubjects.Count) - 1];

        List<Assignment> subjectAssignments = baseStore.Assignments
            .Where(a => a.SubjectId == selectedSubject.Id)
            .ToList();

        Console.WriteLine();
        Console.WriteLine("  Select an assignment or create a new one:");
        for (int i = 0; i < subjectAssignments.Count; i++)
            Console.WriteLine($"  {i + 1}. {subjectAssignments[i].Name} ({subjectAssignments[i].Type})");
        Console.WriteLine($"  {subjectAssignments.Count + 1}. Create new assignment");
        Console.WriteLine();

        int assignmentChoice = MenuHelper.PromptChoice(1, subjectAssignments.Count + 1);
        Assignment selectedAssignment;

        if (assignmentChoice <= subjectAssignments.Count)
        {
            selectedAssignment = subjectAssignments[assignmentChoice - 1];
        }
        else
        {
            selectedAssignment = CreateNewAssignment(selectedSubject.Id);
            baseStore.Assignments.Add(selectedAssignment);
        }

        List<Student> students = baseStore.Users.OfType<Student>().ToList();

        if (students.Count == 0)
        {
            MenuHelper.PrintError("No students registered in the system.");
            MenuHelper.WaitForInput();
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"  Grading: {selectedAssignment.Name} (Max: {selectedAssignment.MaxScore})");
        Console.WriteLine(new string('-', 45));

        List<GradeEntry> gradeEntries = [];

        foreach (Student student in students)
        {
            Console.WriteLine($"  Student: {student.Name}");
            double score = PromptScore(selectedAssignment.MaxScore);
            Console.Write("  Note (optional): ");
            string note = Console.ReadLine()?.Trim() ?? string.Empty;

            gradeEntries.Add(new GradeEntry(
                Guid.NewGuid(),
                selectedAssignment.Id,
                student.Id,
                score,
                note,
                DateTime.Now));

            Console.WriteLine();
        }

        SessionPathCollector collector = new();
        SessionPathResult path = collector.Collect();

        DataStore sessionStore = new()
        {
            Users = baseStore.Users.ToList(),
            Subjects = baseStore.Subjects.ToList(),
            Assignments = [selectedAssignment],
            GradeEntries = gradeEntries
        };

        repository.Save(sessionStore, path.FullPath);
        Console.WriteLine($"  Session saved to: {path.FullPath}");
        MenuHelper.WaitForInput();
    }

    private static Assignment CreateNewAssignment(Guid subjectId)
    {
        Console.Write("  Assignment name: ");
        string name = Console.ReadLine()?.Trim() ?? "Untitled";

        AssignmentType[] types = Enum.GetValues<AssignmentType>();
        Console.WriteLine("  Select assignment type:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");
        Console.WriteLine();
        AssignmentType selectedType = types[MenuHelper.PromptChoice(1, types.Length) - 1];

        Console.Write("  Max score: ");
        double maxScore;
        while (!double.TryParse(Console.ReadLine()?.Trim(), out maxScore) || maxScore <= 0)
            Console.Write("  Enter a positive number for max score: ");

        return new Assignment(
            Guid.NewGuid(),
            subjectId,
            name,
            selectedType,
            DateOnly.FromDateTime(DateTime.Today),
            maxScore);
    }

    private static double PromptScore(double maxScore)
    {
        while (true)
        {
            Console.Write($"  Score (0-{maxScore}): ");
            if (double.TryParse(Console.ReadLine()?.Trim(), out double score) && score >= 0 && score <= maxScore)
                return score;
            Console.WriteLine($"  Score must be between 0 and {maxScore}.");
        }
    }

    private void ViewPreviousSessions()
    {
        MenuHelper.PrintHeader("View Previous Sessions");
        Console.Write("  Enter folder path to scan: ");
        string? folderPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            MenuHelper.PrintError("Folder does not exist.");
            MenuHelper.WaitForInput();
            return;
        }

        string[] files;
        try
        {
            files = Directory.GetFiles(folderPath, "*.json");
        }
        catch (IOException ex)
        {
            MenuHelper.PrintError($"Could not read folder: {ex.Message}");
            MenuHelper.WaitForInput();
            return;
        }

        if (files.Length == 0)
        {
            Console.WriteLine("  No session files found.");
            MenuHelper.WaitForInput();
            return;
        }

        Console.WriteLine();
        foreach (string file in files)
        {
            try
            {
                DataStore store = repository.Load(file);
                Assignment? assignment = store.Assignments.FirstOrDefault();
                if (assignment is null) continue;

                string subjectName = store.Subjects
                    .FirstOrDefault(s => s.Id == assignment.SubjectId)?.Name ?? "Unknown";

                Console.WriteLine($"  File: {Path.GetFileName(file)}");
                Console.WriteLine($"    Assignment: {assignment.Name}");
                Console.WriteLine($"    Subject:    {subjectName}");
                Console.WriteLine($"    Date:       {assignment.Date}");
                Console.WriteLine($"    Students:   {store.GradeEntries.Count}");
                Console.WriteLine();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"  File: {Path.GetFileName(file)} — could not load");
            }
        }

        MenuHelper.WaitForInput();
    }
}
