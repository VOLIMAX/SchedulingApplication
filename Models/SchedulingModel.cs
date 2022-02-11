using Google.OrTools.Sat;
using System.Collections.Generic;

namespace SchedulingApplication.Models
{
    public class SchedulingModel
    {
        public StatisticsModel Statistics { get; set; }
        public IList<List<string>> ListsWithEachSolution { get; set; }
        public double WallTime { get; set; }
        public CpSolverStatus SolverStatus { get; set; }
    }
}
