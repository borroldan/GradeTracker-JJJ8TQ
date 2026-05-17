namespace GradeTracker.Models;

public sealed record SessionPathResult
{
    public required string FolderPath { get; init; }
    public required string FileName { get; init; }
    public string FullPath => Path.Combine(FolderPath, FileName);
}
