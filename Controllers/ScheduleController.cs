using Microsoft.AspNetCore.Mvc;
using SchedulingApplication.Services;
using Microsoft.Extensions.Logging;

namespace SchedulingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ISolutionPrinter _solutionPrinter;
        public ScheduleController(ISolutionPrinter solutionPrinter)
        {
            _solutionPrinter = solutionPrinter;
        }

        [HttpGet]
        public IActionResult GetCalculatedSolutions(int days, int guards, int shifts)
        {
            var schedulingModels = _solutionPrinter.CalculateSolutions(guards, days, shifts);
    
            if (schedulingModels is null || schedulingModels.Solutions.Count == 0)
            {
                //TODO: Add view for the bad request
                return BadRequest("Обчислення повернуло 0 результатів. Спробуйте ввести інші параметри");
            }

            return Ok(schedulingModels);
        }
    }

    
}
