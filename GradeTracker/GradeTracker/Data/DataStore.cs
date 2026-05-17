using GradeTracker.Models;

namespace GradeTracker.Data;

public sealed class DataStore
{
    public List<User> Users { get; init; } = [];
    public List<Subject> Subjects { get; init; } = [];
    public List<Assignment> Assignments { get; init; } = [];
    public List<GradeEntry> GradeEntries { get; init; } = [];
}
