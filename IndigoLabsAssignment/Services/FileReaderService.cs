using IndigoLabsAssignment.Services.Interfaces;

namespace IndigoLabsAssignment.Services
{
    public class FileReaderService : IFileReaderService
    {
        public async IAsyncEnumerable<string> ReadLinesAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(fs);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }
    }
}
