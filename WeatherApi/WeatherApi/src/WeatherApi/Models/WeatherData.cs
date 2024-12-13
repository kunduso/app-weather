namespace WeatherApi.Models
{
    public class WeatherData
    {
        public string Location { get; set; } = string.Empty;
        public double Temperature { get; set; }  // Change from int to double
        public string Unit { get; set; } = string.Empty;
    }
}

