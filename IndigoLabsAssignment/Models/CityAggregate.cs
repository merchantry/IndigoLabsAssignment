namespace IndigoLabsAssignment.Models
{
    public class CityAggregate(double value)
    {
        public double Min { get; set; } = value;
        public double Max { get; set; } = value;
        public double Sum { get; set; } = value;
        public int Count { get; set; } = 1;
    }
}
