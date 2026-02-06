namespace IndigoLabsAssignment.Models
{
    public readonly record struct FileMetaData(
        string Path,
        long Size,
        DateTime LastWriteTimeUtc
    );
}