using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luma.Entities
{
    internal class JobResponse
    {
        public Job Job { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
    internal class Job
    {
        public string Id { get; set; }
        public string ProbeName { get; set; }
    }
}
