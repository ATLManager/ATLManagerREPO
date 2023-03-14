using System.IO;
using System.Threading.Tasks;

public interface IFileManager
{
    Task Save(Stream stream, string filePath);
    Task<Stream> Load(string filePath);
    void Delete(string filePath);
}