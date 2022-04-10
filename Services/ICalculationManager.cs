using SchedulingApplication.Models;

namespace SchedulingApplication.Services
{
    public interface ICalculationManager
    {
        public SchedulingModel CalculateSolutions(int guardsNumber, int daysNumber, int shiftsNumber);
    }
}
