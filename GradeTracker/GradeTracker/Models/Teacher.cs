namespace GradeTracker.Models;

internal sealed record Teacher(Guid Id, string Name, IReadOnlyList<Guid> SubjectIds)
    : User(Id, Name, UserRole.Teacher);
