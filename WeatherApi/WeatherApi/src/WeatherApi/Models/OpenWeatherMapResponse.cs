namespace WeatherApi.Models
{
    public class OpenWeatherMapResponse
    {
        public MainData Main { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }

    public class MainData
    {
        public float Temp { get; set; }
        public float Feels_like { get; set; }
        public float Temp_min { get; set; }
        public float Temp_max { get; set; }
        public int Humidity { get; set; }
    }
}
