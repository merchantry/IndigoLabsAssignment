namespace IndigoLabsAssignment.Services
{
    public interface IFileReaderService
    {
        IAsyncEnumerable<string> ReadLinesAsync(string path);
    }
}
