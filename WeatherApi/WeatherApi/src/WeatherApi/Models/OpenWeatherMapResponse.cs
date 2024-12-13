namespace WeatherApi.Models
{
    public class OpenWeatherMapResponse
    {
        public MainData Main { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }

    public class MainData
    {
        public double Temp { get; set; }  // Change from float to double
        public double Feels_like { get; set; }
        public double Temp_min { get; set; }
        public double Temp_max { get; set; }
        public int Humidity { get; set; }
    }
}
