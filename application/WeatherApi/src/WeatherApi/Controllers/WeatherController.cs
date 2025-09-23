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
                _logger.LogInformation("Received request for location: {Location}", location);
                
                if (string.IsNullOrWhiteSpace(location))
                {
                    _logger.LogWarning("Empty or null location provided");
                    return BadRequest("Location cannot be empty");
                }
                
                var weatherData = await _weatherService.GetWeatherForLocationAsync(location);
                if (weatherData == null)
                {
                    _logger.LogWarning("No weather data returned for location: {Location}", location);
                    return NotFound($"Weather data not found for {location}");
                }
                
                _logger.LogInformation("Successfully returned weather data for {Location}", location);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather for {Location}", location);
                return StatusCode(500, "Error retrieving weather data");
            }
        }
    }
}