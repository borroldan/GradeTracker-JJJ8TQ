using GradeTracker.Data;
using GradeTracker.Models;

namespace GradeTracker.UI;

internal sealed class AdminMenu(DataStore baseStore, IDataStoreRepository repository, string baseFilePath)
{
    public void Run()
    {
        while (true)
        {
            MenuHelper.PrintHeader("Admin Menu");
            Console.WriteLine("  1. Manage Teachers");
            Console.WriteLine("  2. Manage Students");
            Console.WriteLine("  3. Manage Subjects");
            Console.WriteLine("  4. Exit");
            Console.WriteLine();

            switch (MenuHelper.PromptChoice(1, 4))
            {
                case 1: ManageTeachers(); break;
                case 2: ManageStudents(); break;
                case 3: ManageSubjects(); break;
                case 4: return;
            }
        }
    }

    private void ManageTeachers()
    {
        MenuHelper.PrintHeader("Manage Teachers");
        List<Teacher> teachers = baseStore.Users.OfType<Teacher>().ToList();

        for (int i = 0; i < teachers.Count; i++)
            Console.WriteLine($"  {i + 1}. {teachers[i].Name}");

        if (teachers.Count == 0)
            Console.WriteLine("  (no teachers)");

        Console.WriteLine();
        Console.WriteLine("  1. Add Teacher");
        Console.WriteLine("  2. Delete Teacher");
        Console.WriteLine("  3. Back");
        Console.WriteLine();

        switch (MenuHelper.PromptChoice(1, 3))
        {
            case 1:
                Console.Write("  Enter teacher name: ");
                string? teacherName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(teacherName))
                {
                    MenuHelper.PrintError("Name cannot be empty.");
                    break;
                }
                baseStore.Users.Add(new Teacher(Guid.NewGuid(), teacherName, []));
                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Teacher '{teacherName}' added successfully.");
                break;

            case 2:
                if (teachers.Count == 0)
                {
                    MenuHelper.PrintError("No teachers to delete.");
                    break;
                }
                Console.WriteLine("  Select teacher to delete:");
                int deleteChoice = MenuHelper.PromptChoice(1, teachers.Count);
                Teacher toRemove = teachers[deleteChoice - 1];
                baseStore.Users.Remove(toRemove);
                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Teacher '{toRemove.Name}' removed.");
                break;
        }

        MenuHelper.WaitForInput();
    }

    private void ManageStudents()
    {
        MenuHelper.PrintHeader("Manage Students");
        List<Student> students = baseStore.Users.OfType<Student>().ToList();

        for (int i = 0; i < students.Count; i++)
            Console.WriteLine($"  {i + 1}. {students[i].Name}");

        if (students.Count == 0)
            Console.WriteLine("  (no students)");

        Console.WriteLine();
        Console.WriteLine("  1. Add Student");
        Console.WriteLine("  2. Delete Student");
        Console.WriteLine("  3. Back");
        Console.WriteLine();

        switch (MenuHelper.PromptChoice(1, 3))
        {
            case 1:
                Console.Write("  Enter student name: ");
                string? studentName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(studentName))
                {
                    MenuHelper.PrintError("Name cannot be empty.");
                    break;
                }
                baseStore.Users.Add(new Student(Guid.NewGuid(), studentName));
                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Student '{studentName}' added successfully.");
                break;

            case 2:
                if (students.Count == 0)
                {
                    MenuHelper.PrintError("No students to delete.");
                    break;
                }
                Console.WriteLine("  Select student to delete:");
                int deleteChoice = MenuHelper.PromptChoice(1, students.Count);
                Student toRemove = students[deleteChoice - 1];
                baseStore.Users.Remove(toRemove);
                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Student '{toRemove.Name}' removed.");
                break;
        }

        MenuHelper.WaitForInput();
    }

    private void ManageSubjects()
    {
        MenuHelper.PrintHeader("Manage Subjects");
        List<Subject> subjects = baseStore.Subjects;

        for (int i = 0; i < subjects.Count; i++)
        {
            Subject subject = subjects[i];
            string teacherName = baseStore.Users
                .OfType<Teacher>()
                .FirstOrDefault(t => t.Id == subject.TeacherId)?.Name ?? "Unassigned";
            Console.WriteLine($"  {i + 1}. {subject.Name} (Teacher: {teacherName})");
        }

        if (subjects.Count == 0)
            Console.WriteLine("  (no subjects)");

        Console.WriteLine();
        Console.WriteLine("  1. Add Subject");
        Console.WriteLine("  2. Delete Subject");
        Console.WriteLine("  3. Back");
        Console.WriteLine();

        switch (MenuHelper.PromptChoice(1, 3))
        {
            case 1:
                Console.Write("  Enter subject name: ");
                string? subjectName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(subjectName))
                {
                    MenuHelper.PrintError("Name cannot be empty.");
                    break;
                }

                List<Teacher> teachers = baseStore.Users.OfType<Teacher>().ToList();
                if (teachers.Count == 0)
                {
                    MenuHelper.PrintError("No teachers available to assign.");
                    break;
                }

                Console.WriteLine("  Select a teacher to assign:");
                for (int i = 0; i < teachers.Count; i++)
                    Console.WriteLine($"  {i + 1}. {teachers[i].Name}");
                Console.WriteLine();

                int teacherChoice = MenuHelper.PromptChoice(1, teachers.Count);
                Teacher selectedTeacher = teachers[teacherChoice - 1];
                Guid newSubjectId = Guid.NewGuid();

                baseStore.Subjects.Add(new Subject(newSubjectId, subjectName, selectedTeacher.Id));

                int teacherIndex = baseStore.Users.FindIndex(u => u.Id == selectedTeacher.Id);
                baseStore.Users[teacherIndex] = selectedTeacher with
                {
                    SubjectIds = selectedTeacher.SubjectIds.Append(newSubjectId).ToList()
                };

                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Subject '{subjectName}' added and assigned to {selectedTeacher.Name}.");
                break;

            case 2:
                if (subjects.Count == 0)
                {
                    MenuHelper.PrintError("No subjects to delete.");
                    break;
                }
                Console.WriteLine("  Select subject to delete:");
                int deleteChoice = MenuHelper.PromptChoice(1, subjects.Count);
                Subject toRemove = subjects[deleteChoice - 1];
                baseStore.Subjects.Remove(toRemove);

                int ownerIndex = baseStore.Users.FindIndex(u => u.Id == toRemove.TeacherId);
                if (ownerIndex >= 0 && baseStore.Users[ownerIndex] is Teacher owner)
                {
                    baseStore.Users[ownerIndex] = owner with
                    {
                        SubjectIds = owner.SubjectIds.Where(id => id != toRemove.Id).ToList()
                    };
                }

                repository.Save(baseStore, baseFilePath);
                Console.WriteLine($"  Subject '{toRemove.Name}' removed.");
                break;
        }

        MenuHelper.WaitForInput();
    }
}
