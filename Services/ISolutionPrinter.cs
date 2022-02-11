using SchedulingApplication.Models;

namespace SchedulingApplication.Services
{
    public interface ISolutionPrinter
    {
        public SchedulingModel CalculateSolutions(int guardsNumber, int daysNumber, int shiftsNumber);
    }
}
