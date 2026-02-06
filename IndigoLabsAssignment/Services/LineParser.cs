using System.Globalization;
using IndigoLabsAssignment.Services.Interfaces;

namespace IndigoLabsAssignment.Services
{
    public class LineParser : ILineParser
    {
        public bool TryParse(string line, out string city, out double temperature)
        {
            city = string.Empty;
            temperature = default;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            int separatorIndex = line.LastIndexOf(';');
            if (separatorIndex == -1 || separatorIndex == line.Length - 1)
                return false;

            city = line.Substring(0, separatorIndex).Trim();
            string tempPart = line.Substring(separatorIndex + 1).Trim();

            if (
                !double.TryParse(
                    tempPart,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out temperature
                )
            )
                return false;

            return true;
        }
    }
}
