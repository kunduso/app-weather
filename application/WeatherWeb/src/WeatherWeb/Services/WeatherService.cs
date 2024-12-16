using WeatherWeb.Models;

namespace WeatherWeb.Services
{
    public class WeatherService
    {
        private readonly HttpClient _client;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient client, ILogger<WeatherService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<WeatherData?> GetWeatherForLocation(string location)
        {
            try
            {
                var baseUrl = Environment.GetEnvironmentVariable("WEATHER_API_BASE_URL") ?? "http://app-weather-api:8080";
                var requestUrl = $"{baseUrl}/weather/{location}";
                _logger.LogInformation($"Attempting to call API at: {requestUrl}");

                var response = await _client.GetAsync(requestUrl);
                
                _logger.LogInformation($"API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"API Response content: {content}");
                    return await response.Content.ReadFromJsonAsync<WeatherData>();
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error fetching weather: Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, 
                    errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling weather service");
                throw;
            }
        }
    }
}
