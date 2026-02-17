using IndigoLabsAssignment.Services.Interfaces;

namespace IndigoLabsAssignment.Services
{
    public class FileReaderService : IFileReaderService
    {
        public async IAsyncEnumerable<string> ReadLinesAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            using var fileStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );
            using var streamReader = new StreamReader(fileStream);

            string? line;
            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }
    }
}
