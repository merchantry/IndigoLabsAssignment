namespace IndigoLabsAssignment.Services.Interfaces
{
    public interface IFileReaderService
    {
        IAsyncEnumerable<string> ReadLinesAsync(string path);
    }
}
