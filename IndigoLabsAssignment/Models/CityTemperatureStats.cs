namespace IndigoLabsAssignment.Models
{
    public class CityTemperatureStats
    {
        public string City { get; set; } = string.Empty;
        public double Min { get; set; }
        public double Max { get; set; }
        public double AvgTemp { get; set; }
        public int Count { get; set; }
    }
}
