namespace IndigoLabsAssignment.Services.Interfaces
{
    public interface ILineParser
    {
        bool TryParse(string line, out string city, out double temperature);
    }
}
