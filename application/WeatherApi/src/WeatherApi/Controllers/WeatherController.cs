using Microsoft.AspNetCore.Mvc;
using WeatherApi.Services;
using WeatherApi.Models;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet("{location}")]
        public async Task<ActionResult<WeatherData>> GetWeather(string location)
        {
            try
            {
                // Sanitize location for logging to prevent log injection attacks
                var sanitizedLocation = SanitizeForLogging(location);
                _logger.LogInformation("Received request for location: {Location}", sanitizedLocation);
                
                if (string.IsNullOrWhiteSpace(location))
                {
                    _logger.LogWarning("Empty or null location provided");
                    return BadRequest("Location cannot be empty");
                }
                
                // Validate location contains only safe characters
                if (location.Any(c => char.IsControl(c) && c != ' '))
                {
                    _logger.LogWarning("Invalid characters detected in location parameter");
                    return BadRequest("Location contains invalid characters");
                }
                
                var weatherData = await _weatherService.GetWeatherForLocationAsync(location);
                if (weatherData == null)
                {
                    _logger.LogWarning("No weather data returned for location: {Location}", sanitizedLocation);
                    return NotFound($"Weather data not found for {location}");
                }
                
                _logger.LogInformation("Successfully returned weather data for {Location}", sanitizedLocation);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                var sanitizedLocation = SanitizeForLogging(location);
                _logger.LogError(ex, "Error getting weather for {Location}", sanitizedLocation);
                return StatusCode(500, "Error retrieving weather data");
            }
        }
        
        private static string SanitizeForLogging(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return "[null/empty]";
                
            // Remove control characters and limit length for logging
            var sanitized = new string(input.Where(c => !char.IsControl(c) || c == ' ')
                                           .Take(100)
                                           .ToArray());
                                           
            return string.IsNullOrEmpty(sanitized) ? "[invalid]" : sanitized;
        }
    }
}