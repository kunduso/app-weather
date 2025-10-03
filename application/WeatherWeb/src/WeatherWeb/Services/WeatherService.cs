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
            _client.Timeout = TimeSpan.FromSeconds(30);
            _logger = logger;
        }

        public async Task<WeatherData?> GetWeatherForLocation(string location)
        {
            try
            {
                var sanitizedLocation = SanitizeForLogging(location);
                var baseUrl = Environment.GetEnvironmentVariable("WEATHER_API_BASE_URL") ?? "http://app-weather-api:8080";
                var requestUrl = $"{baseUrl}/weather/{location}";
                _logger.LogInformation("Attempting to call API for location: {Location}", sanitizedLocation);

                var response = await _client.GetAsync(requestUrl);
                
                _logger.LogInformation("API Response Status: {StatusCode} for location: {Location}", response.StatusCode, sanitizedLocation);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sanitizedContent = SanitizeForLogging(content);
                    _logger.LogInformation("API Response content: {Content}", sanitizedContent);
                    return await response.Content.ReadFromJsonAsync<WeatherData>();
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                var sanitizedErrorContent = SanitizeForLogging(errorContent);
                _logger.LogError("Error fetching weather: Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, 
                    sanitizedErrorContent);
                return null;
            }
            catch (Exception ex)
            {
                var sanitizedLocation = SanitizeForLogging(location);
                _logger.LogError(ex, "Exception while calling weather service for location: {Location}", sanitizedLocation);
                throw;
            }
        }
        
        private static string SanitizeForLogging(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return "[null/empty]";
                
            // Remove control characters and limit length for logging
            var sanitized = new string(input.Where(c => !char.IsControl(c) || c == ' ')
                                           .Take(200)
                                           .ToArray());
                                           
            return string.IsNullOrEmpty(sanitized) ? "[invalid]" : sanitized;
        }
    }
}
