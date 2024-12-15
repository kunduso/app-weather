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
                Weather = await _weatherService.GetWeatherForLocation(Location);
                if (Weather == null)
                {
                    ErrorMessage = $"Unable to fetch weather for {Location}";
                }
            }
        }
    }
}
