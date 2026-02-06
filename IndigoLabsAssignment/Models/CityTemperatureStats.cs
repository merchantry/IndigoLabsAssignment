namespace IndigoLabsAssignment.Models
{
    public readonly struct CityTemperatureStats(string city, double min, double max, double sum, int count)
    {
        public string City { get; } = city;
        public double Min { get; } = min;
        public double Max { get; } = max;
        public double AvgTemp { get; } = sum / count;
        public int Count { get; } = count;
    }
}
