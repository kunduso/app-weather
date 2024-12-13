using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherData?> GetWeatherForLocationAsync(string location);
    }
}
