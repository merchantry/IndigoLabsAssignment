namespace IndigoLabsAssignment.Services
{
    // Responsible for parsing a single line of the input file.
    public interface ILineParser
    {
        // Try to parse a line in the form "City;Temperature".
        // Returns true when successful and outputs city and temperature.
        bool TryParse(string line, out string city, out double temperature);
    }
}
