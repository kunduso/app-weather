using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherWeb.Models;
using WeatherWeb.Services;

namespace WeatherWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WeatherService _weatherService;
        private readonly ILogger<IndexModel> _logger;

        public string Location { get; set; }
        
        public WeatherData? Weather { get; set; }
        public string? ErrorMessage { get; set; }

        public IndexModel(WeatherService weatherService, ILogger<IndexModel> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        public async Task OnGet(string? location = null)
        {
            if (!string.IsNullOrEmpty(location))
            {
                Location = location;
                try
                {
                    Weather = await _weatherService.GetWeatherForLocation(location);
                    if (Weather == null)
                    {
                        ErrorMessage = $"Unable to fetch weather for {SanitizeForDisplay(location)}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting weather for location: {Location}", SanitizeForDisplay(location));
                    ErrorMessage = $"Error retrieving weather data for {SanitizeForDisplay(location)}";
                }
            }
        }


        
        private static string SanitizeForDisplay(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return "[invalid location]";
                
            // Remove control characters and limit length for display
            var sanitized = new string(input.Where(c => !char.IsControl(c) || c == ' ')
                                           .Take(50)
                                           .ToArray());
                                           
            return string.IsNullOrEmpty(sanitized) ? "[invalid location]" : sanitized;
        }
    }
}
