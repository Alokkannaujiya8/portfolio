using System.IO;
using System.Threading.Tasks;

namespace Portfolio.Application.Interfaces.Storage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
    void DeleteFile(string relativePath);
}
