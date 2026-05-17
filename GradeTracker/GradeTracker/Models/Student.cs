namespace GradeTracker.Models;

internal sealed record Student(Guid Id, string Name)
    : User(Id, Name, UserRole.Student);
