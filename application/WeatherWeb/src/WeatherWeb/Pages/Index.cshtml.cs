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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Location))
            {
                try
                {
                    // Validate location contains only safe characters
                    if (Location.Any(c => char.IsControl(c) && c != ' '))
                    {
                        ErrorMessage = "Location contains invalid characters";
                        return Page();
                    }
                    
                    _logger.LogInformation("Processing weather request for location: {Location}", SanitizeForDisplay(Location));
                    
                    Weather = await _weatherService.GetWeatherForLocation(Location);
                    if (Weather == null)
                    {
                        var sanitizedLocation = SanitizeForDisplay(Location);
                        ErrorMessage = $"Unable to fetch weather for {sanitizedLocation}";
                        _logger.LogWarning("No weather data returned for location: {Location}", sanitizedLocation);
                        return Page();
                    }
                    else
                    {
                        _logger.LogInformation("Successfully retrieved weather data for location: {Location}", SanitizeForDisplay(Location));
                        // Redirect after successful POST to prevent form resubmission
                        return RedirectToPage(new { location = Location });
                    }
                }
                catch (Exception ex)
                {
                    var sanitizedLocation = SanitizeForDisplay(Location);
                    _logger.LogError(ex, "Error processing weather request for location: {Location}", sanitizedLocation);
                    
                    // Provide more specific error messages based on exception type
                    ErrorMessage = ex switch
                    {
                        HttpRequestException => $"Unable to connect to weather service for {sanitizedLocation}. Please check your internet connection and try again.",
                        TaskCanceledException => $"Request timed out while getting weather data for {sanitizedLocation}. Please try again.",
                        _ => $"Error retrieving weather data for {sanitizedLocation}. Please try again."
                    };
                    return Page();
                }
            }
            return Page();
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
