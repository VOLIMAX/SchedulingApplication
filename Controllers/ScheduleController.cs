using Microsoft.AspNetCore.Mvc;
using SchedulingApplicaiton.Controllers;
using SchedulingApplication.Services;
using Microsoft.Extensions.Logging;

namespace SchedulingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ISolutionPrinter _solutionPrinter;
        private readonly ILogger<WeatherForecastController> _logger;
        public ScheduleController(ISolutionPrinter solutionPrinter, ILogger<WeatherForecastController> logger)
        {
            _solutionPrinter = solutionPrinter;
            _logger = logger;
        }

        [HttpGet]
        //[Route("/Get")]
        public IActionResult GetCalculatedSolutions(int days, int guards, int shifts)
        {
            var solutions = _solutionPrinter.CalculateSolutions(guards, days, shifts);

            if (solutions is null)
            {
                _logger.LogError("Solutions were null");
                return BadRequest("Solutions were null");
            }

            return Ok(solutions);
        }
       
        // GET api/<ScheduleController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}        
    }

    
}
