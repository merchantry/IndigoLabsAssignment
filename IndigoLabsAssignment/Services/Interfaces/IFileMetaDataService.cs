using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services.Interfaces
{
    public interface IFileMetaDataService
    {
        FileMetaData FromPath(string path);
    }
}
