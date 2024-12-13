using WeatherApi.Models;
using System.Text.Json;

namespace WeatherApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["OpenWeatherMap:ApiKey"] 
                ?? throw new ArgumentNullException("OpenWeatherMap API key is not configured");
        }

        public async Task<WeatherData?> GetWeatherForLocationAsync(string location)
        {
            try
            {
                _logger.LogInformation("Fetching weather data for {Location}", location);

                var url = $"http://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid={_apiKey}";
                _logger.LogInformation("Calling OpenWeatherMap API: {Url}", url);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response: {Content}", content);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(content, options);

                if (weatherResponse == null)
                {
                    _logger.LogError("Failed to deserialize weather data");
                    return null;
                }

                var weatherData = new WeatherData
                {
                    Location = weatherResponse.Name ?? location,
                    Temperature = Math.Round(weatherResponse.Main?.Temp ?? 0, 1),
                    Unit = "Celsius"
                };

                _logger.LogInformation("Processed weather data for {Location}: {Temperature}°C", 
                    weatherData.Location, weatherData.Temperature);

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
