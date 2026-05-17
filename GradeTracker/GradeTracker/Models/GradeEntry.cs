namespace GradeTracker.Models;

internal sealed record GradeEntry(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    double Score,
    string Note,
    DateTime GradedAt);
