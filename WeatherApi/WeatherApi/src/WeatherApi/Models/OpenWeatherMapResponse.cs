namespace WeatherApi.Models
{
    public class OpenWeatherMapResponse
    {
        public MainData? Main { get; set; }
        public string? Name { get; set; }
    }

    public class MainData
    {
        public double Temp { get; set; }
        public double Feels_like { get; set; }
        public double Temp_min { get; set; }
        public double Temp_max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }
}
