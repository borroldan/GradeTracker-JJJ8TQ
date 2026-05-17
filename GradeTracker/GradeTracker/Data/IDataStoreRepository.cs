namespace GradeTracker.Data;

public interface IDataStoreRepository
{
    DataStore Load(string filePath);
    void Save(DataStore store, string filePath);
    IEnumerable<string> FindFilesContainingStudent(string folderPath, Guid studentId);
}
