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
                var weatherData = await _weatherService.GetWeatherForLocationAsync(location);
                if (weatherData == null)
                {
                    return NotFound();
                }
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