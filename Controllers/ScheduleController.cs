using Microsoft.AspNetCore.Mvc;
using SchedulingApplication.Services;
using System.Collections.Generic;

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
        [Route("/Get")]
        public IEnumerable<object> Get(int guardsNumber, int daysNumber, int shiftsNumber)
        {
            var solutions = _solutionPrinter.CalculateSolutions(guardsNumber, daysNumber, shiftsNumber);
            return solutions;
        }
       
        // GET api/<ScheduleController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }        
    }

    
}
