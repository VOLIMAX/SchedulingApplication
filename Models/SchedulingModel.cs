using System.Collections.Generic;

namespace SchedulingApplication.Models
{
    public class SchedulingModel
    {
        public StatisticsModel Statistics { get; set; }
        public IList<List<string>> ListsWithEachSolution { get; set; }
    }
}
