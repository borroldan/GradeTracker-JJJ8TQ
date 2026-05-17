using System.Text.Json;

namespace GradeTracker.Data;

internal sealed class DataStoreRepository : IDataStoreRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public DataStore Load(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Session file not found: {filePath}");

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DataStore>(json, SerializerOptions)
                   ?? throw new InvalidOperationException($"Deserialization returned null for: {filePath}");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Malformed JSON in session file: {filePath}", ex);
        }
    }

    public void Save(DataStore store, string filePath)
    {
        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(store, SerializerOptions);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving session file '{filePath}': {ex.Message}");
            throw;
        }
    }

    public IEnumerable<string> FindFilesContainingStudent(string folderPath, Guid studentId)
    {
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Folder not found: {folderPath}");
            yield break;
        }

        foreach (string file in Directory.EnumerateFiles(folderPath, "*.json"))
        {
            DataStore? store = null;
            try
            {
                string json = File.ReadAllText(file);
                store = JsonSerializer.Deserialize<DataStore>(json, SerializerOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skipping unreadable file '{file}': {ex.Message}");
            }

            if (store?.GradeEntries.Any(e => e.StudentId == studentId) == true)
                yield return file;
        }
    }
}
