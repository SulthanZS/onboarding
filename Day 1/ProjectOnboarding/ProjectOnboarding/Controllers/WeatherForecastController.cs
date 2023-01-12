using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace ProjectOnboarding.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(404)]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// Retrieves weather forecast by summary
        /// </summary>
        /// <param name="summary">The Summary</param>
        /// <response code="200">OK</response>
        /// <response code="404">not found</response>
        [HttpGet]
        [Route("{summary}")]
        [ProducesResponseType(404)]
        public ActionResult Get([FromRoute] string summary)
        {
            var listForecast = Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray().Where(x => x.Summary == summary);

            if (listForecast.Any())
            {
                return new OkObjectResult(listForecast);
            }
            return new NotFoundResult();
             
        }

        /// <summary>
        /// Add summary temp (not permanent)
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult AddSummaryTemp([FromBody] string summary)
        {
            if (!Summaries.Contains(summary))
            {
                Summaries.Append(summary);
                return new OkObjectResult(Summaries);
            }
            return new OkObjectResult(Summaries);
        }

        /// <summary>
        /// Delete summary temp (not permanent)
        /// </summary>
        /// <param name="summary">The Summary</param>
        /// <response code="200">OK</response>
        /// <response code="400">bad request</response>
        [HttpDelete]
        [Route("{summary}")]
        [ProducesResponseType(200)]
        public ActionResult DeleteSummaryTemp([FromRoute] string summary)
        {
            return new OkObjectResult(Summaries.ToList().Remove(summary));
        }
    }
}