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

        [BindProperty]
        public string Location { get; set; }
        
        public WeatherData? Weather { get; set; }
        public string? ErrorMessage { get; set; }

        public IndexModel(WeatherService weatherService, ILogger<IndexModel> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Location))
            {
                // Validate location contains only safe characters
                if (Location.Any(c => char.IsControl(c) && c != ' '))
                {
                    ErrorMessage = "Location contains invalid characters";
                    return;
                }
                
                Weather = await _weatherService.GetWeatherForLocation(Location);
                if (Weather == null)
                {
                    var sanitizedLocation = SanitizeForDisplay(Location);
                    ErrorMessage = $"Unable to fetch weather for {sanitizedLocation}";
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
