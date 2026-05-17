namespace GradeTracker.Models;

internal sealed record Admin(Guid Id, string Name)
    : User(Id, Name, UserRole.Admin);
