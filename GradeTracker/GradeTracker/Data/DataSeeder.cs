using GradeTracker.Models;

namespace GradeTracker.Data;

internal sealed class DataSeeder
{
    public void Seed(string filePath)
    {
        if (File.Exists(filePath))
            return;

        var adminId = Guid.NewGuid();
        var teacherSmithId = Guid.NewGuid();
        var teacherJohnsonId = Guid.NewGuid();
        var studentAliceId = Guid.NewGuid();
        var studentBobId = Guid.NewGuid();
        var studentCarolId = Guid.NewGuid();
        var studentDavidId = Guid.NewGuid();

        var subjectMathId = Guid.NewGuid();
        var subjectPhysicsId = Guid.NewGuid();
        var subjectLiteratureId = Guid.NewGuid();

        var mathMidtermId = Guid.NewGuid();
        var mathHomeworkId = Guid.NewGuid();
        var physicsQuizId = Guid.NewGuid();
        var physicsProjectId = Guid.NewGuid();
        var litEssayId = Guid.NewGuid();
        var litQuizId = Guid.NewGuid();

        DataStore store = new()
        {
            Users =
            [
                new Admin(adminId, "Admin"),
                new Teacher(teacherSmithId, "Mr. Smith", [subjectMathId, subjectPhysicsId]),
                new Teacher(teacherJohnsonId, "Ms. Johnson", [subjectLiteratureId]),
                new Student(studentAliceId, "Alice Brown"),
                new Student(studentBobId, "Bob Wilson"),
                new Student(studentCarolId, "Carol Davis"),
                new Student(studentDavidId, "David Miller")
            ],
            Subjects =
            [
                new Subject(subjectMathId, "Mathematics", teacherSmithId),
                new Subject(subjectPhysicsId, "Physics", teacherSmithId),
                new Subject(subjectLiteratureId, "Literature", teacherJohnsonId)
            ],
            Assignments =
            [
                new Assignment(mathMidtermId, subjectMathId, "Midterm Exam", AssignmentType.Exam, new DateOnly(2026, 3, 15), 100),
                new Assignment(mathHomeworkId, subjectMathId, "Algebra Homework", AssignmentType.Homework, new DateOnly(2026, 3, 20), 50),
                new Assignment(physicsQuizId, subjectPhysicsId, "Mechanics Quiz", AssignmentType.Quiz, new DateOnly(2026, 3, 18), 40),
                new Assignment(physicsProjectId, subjectPhysicsId, "Lab Report Project", AssignmentType.Project, new DateOnly(2026, 4, 1), 80),
                new Assignment(litEssayId, subjectLiteratureId, "Shakespeare Essay", AssignmentType.Homework, new DateOnly(2026, 3, 22), 60),
                new Assignment(litQuizId, subjectLiteratureId, "Poetry Analysis Quiz", AssignmentType.Quiz, new DateOnly(2026, 3, 25), 30)
            ],
            GradeEntries =
            [
                new GradeEntry(Guid.NewGuid(), mathMidtermId, studentAliceId, 92, "Excellent work", new DateTime(2026, 3, 15, 10, 0, 0)),
                new GradeEntry(Guid.NewGuid(), mathMidtermId, studentBobId, 78, "Good effort", new DateTime(2026, 3, 15, 10, 5, 0)),
                new GradeEntry(Guid.NewGuid(), mathMidtermId, studentCarolId, 85, "Well done", new DateTime(2026, 3, 15, 10, 10, 0)),
                new GradeEntry(Guid.NewGuid(), mathMidtermId, studentDavidId, 63, "Needs improvement", new DateTime(2026, 3, 15, 10, 15, 0)),

                new GradeEntry(Guid.NewGuid(), mathHomeworkId, studentAliceId, 48, "Nearly perfect", new DateTime(2026, 3, 20, 14, 0, 0)),
                new GradeEntry(Guid.NewGuid(), mathHomeworkId, studentBobId, 35, "Missing steps shown", new DateTime(2026, 3, 20, 14, 5, 0)),
                new GradeEntry(Guid.NewGuid(), mathHomeworkId, studentCarolId, 42, "Good approach", new DateTime(2026, 3, 20, 14, 10, 0)),
                new GradeEntry(Guid.NewGuid(), mathHomeworkId, studentDavidId, 28, "Incomplete solutions", new DateTime(2026, 3, 20, 14, 15, 0)),

                new GradeEntry(Guid.NewGuid(), physicsQuizId, studentAliceId, 36, "Strong understanding", new DateTime(2026, 3, 18, 9, 0, 0)),
                new GradeEntry(Guid.NewGuid(), physicsQuizId, studentBobId, 30, "Solid performance", new DateTime(2026, 3, 18, 9, 5, 0)),
                new GradeEntry(Guid.NewGuid(), physicsQuizId, studentCarolId, 25, "Review formulas", new DateTime(2026, 3, 18, 9, 10, 0)),
                new GradeEntry(Guid.NewGuid(), physicsQuizId, studentDavidId, 38, "Outstanding recall", new DateTime(2026, 3, 18, 9, 15, 0)),

                new GradeEntry(Guid.NewGuid(), physicsProjectId, studentAliceId, 72, "Great lab report", new DateTime(2026, 4, 1, 16, 0, 0)),
                new GradeEntry(Guid.NewGuid(), physicsProjectId, studentBobId, 60, "Adequate analysis", new DateTime(2026, 4, 1, 16, 5, 0)),
                new GradeEntry(Guid.NewGuid(), physicsProjectId, studentCarolId, 68, "Good methodology", new DateTime(2026, 4, 1, 16, 10, 0)),
                new GradeEntry(Guid.NewGuid(), physicsProjectId, studentDavidId, 75, "Excellent conclusions", new DateTime(2026, 4, 1, 16, 15, 0)),

                new GradeEntry(Guid.NewGuid(), litEssayId, studentAliceId, 55, "Insightful analysis", new DateTime(2026, 3, 22, 11, 0, 0)),
                new GradeEntry(Guid.NewGuid(), litEssayId, studentBobId, 42, "Solid arguments", new DateTime(2026, 3, 22, 11, 5, 0)),
                new GradeEntry(Guid.NewGuid(), litEssayId, studentCarolId, 58, "Exceptional writing", new DateTime(2026, 3, 22, 11, 10, 0)),
                new GradeEntry(Guid.NewGuid(), litEssayId, studentDavidId, 38, "Needs more depth", new DateTime(2026, 3, 22, 11, 15, 0)),

                new GradeEntry(Guid.NewGuid(), litQuizId, studentAliceId, 27, "Well prepared", new DateTime(2026, 3, 25, 8, 0, 0)),
                new GradeEntry(Guid.NewGuid(), litQuizId, studentBobId, 22, "Decent recall", new DateTime(2026, 3, 25, 8, 5, 0)),
                new GradeEntry(Guid.NewGuid(), litQuizId, studentCarolId, 29, "Near perfect", new DateTime(2026, 3, 25, 8, 10, 0)),
                new GradeEntry(Guid.NewGuid(), litQuizId, studentDavidId, 18, "Study more poetry", new DateTime(2026, 3, 25, 8, 15, 0))
            ]
        };

        DataStoreRepository repository = new();
        repository.Save(store, filePath);
    }
}
