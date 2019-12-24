using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreateWorkItems.Models
{
    public class WorkItem
    {
        public string ProjectName { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string Description { get; set; }
    }
}
