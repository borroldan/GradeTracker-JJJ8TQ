namespace GradeTracker.Models;

public sealed record SessionPathResult
{
    public required string FolderPath { get; init; }

    private readonly string _fileName = string.Empty;

    public required string FileName
    {
        get => _fileName;
        init => _fileName = value.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? value
            : $"{value}.json";
    }

    public string FullPath => Path.Combine(FolderPath, FileName);
}
