using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CASS___Construction_Assistance.Models
{
    public class APIresult
    {
        public List<APIusers> users { get; set; }
        public List<Project> projects { get; set; }
        public List<Project> finishedProjects { get; set; }
    }
}
