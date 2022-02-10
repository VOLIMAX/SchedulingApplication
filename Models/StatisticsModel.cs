using Google.OrTools.Sat;

namespace SchedulingApplication.Models
{
    public class StatisticsModel
    {
        public long Conflicts { get; set; }
        public long Branches { get; set; }
        public double WallTime { get; set; }
        public CpSolverStatus SolverStatus { get; set; }
    }
}
