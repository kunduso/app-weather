using WeatherApi.Models;

namespace WeatherApi.Services
{
    public interface IWeatherService
    {
        Task<WeatherData?> GetWeatherForLocationAsync(string location);
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<WeatherData?> GetWeatherForLocationAsync(string location)
        {
            try
            {
                // Here you would typically call a weather API service
                // This is a placeholder implementation
                var weatherData = new WeatherData
                {
                    Location = location,
                    Temperature = 20, // Replace with actual API call
                    Unit = "Celsius"
                };

                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data for {Location}", location);
                return null;
            }
        }
    }
}
