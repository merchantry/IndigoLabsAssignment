using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services.Interfaces;

namespace IndigoLabsAssignment.Services
{
    public class FileMetaDataService : IFileMetaDataService
    {
        public FileMetaData FromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path must not be null or empty.", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}", path);

            var info = new FileInfo(path);

            return new FileMetaData
            {
                Path = info.FullName,
                Size = info.Length,
                LastWriteTimeUtc = info.LastWriteTimeUtc
            };
        }
    }
}
