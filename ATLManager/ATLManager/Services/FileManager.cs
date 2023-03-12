using ATLManager.Services;
using System.IO;
using System.Threading.Tasks;

public class FileManager : IFileManager
{
    public async Task Save(Stream stream, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);
    }

    public async Task<Stream> Load(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        fileStream.Dispose();
        memoryStream.Position = 0;
        return memoryStream;
    }

    public void Delete(string filePath)
    {
        File.Delete(filePath);
    }
}