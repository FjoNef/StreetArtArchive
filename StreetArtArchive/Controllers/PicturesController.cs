using Microsoft.AspNetCore.Mvc;

namespace StreetArtArchive.Controllers;

[ApiController]
[Route("[controller]")]
public class PicturesController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<PicturesController> _logger;

    public PicturesController(ILogger<PicturesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public object Get(int page)
    {
        Thread.Sleep(1000);
        var weather = Enumerable.Range(1, 9).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        return new { pictures = weather, hasMore = true };
    }
}