namespace GradeTracker.Models;

internal sealed record Assignment(
    Guid Id,
    Guid SubjectId,
    string Name,
    AssignmentType Type,
    DateOnly Date,
    double MaxScore);
