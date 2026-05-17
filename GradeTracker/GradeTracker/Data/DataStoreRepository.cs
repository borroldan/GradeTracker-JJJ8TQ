using System.Text.Json;

namespace GradeTracker.Data;

internal sealed class DataStoreRepository : IDataStoreRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public DataStore Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new InvalidOperationException($"Session file does not exist: {filePath}");

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DataStore>(json, SerializerOptions) ?? new DataStore();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to parse JSON in '{filePath}': {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error reading '{filePath}': {ex.Message}");
            throw;
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
        catch (JsonException ex)
        {
            Console.WriteLine($"Serialization error for '{filePath}': {ex.Message}");
            throw;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error writing '{filePath}': {ex.Message}");
            throw;
        }
    }

    public IEnumerable<string> FindFilesContainingStudent(string folderPath, Guid studentId)
    {
        string[] files;

        try
        {
            files = Directory.GetFiles(folderPath, "*.json");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"Folder not found: {folderPath}");
            yield break;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O error accessing folder '{folderPath}': {ex.Message}");
            yield break;
        }

        foreach (string file in files)
        {
            DataStore? store;
            try
            {
                string json = File.ReadAllText(file);
                store = JsonSerializer.Deserialize<DataStore>(json, SerializerOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Skipping malformed JSON file '{file}': {ex.Message}");
                continue;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Skipping unreadable file '{file}': {ex.Message}");
                continue;
            }

            if (store?.GradeEntries.Any(entry => entry.StudentId == studentId) == true)
                yield return file;
        }
    }
}
