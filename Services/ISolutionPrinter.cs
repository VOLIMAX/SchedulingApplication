using System.Collections.Generic;

namespace SchedulingApplication.Services
{
    public interface ISolutionPrinter
    {
        public IEnumerable<object> CalculateSolutions(int guardsNumber, int daysNumber, int shiftsNumber);
    }
}
