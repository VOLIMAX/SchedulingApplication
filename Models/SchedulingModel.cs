using System.Collections.Generic;

namespace SchedulingApplication.Models
{
    public class SchedulingModel
    {
        public StatisticsModel Statistics { get; set; }
        public IList<List<string>> SolutionsInfoLists { get; set; }
        public List<string> Solutions { get; set; }
    }
}
