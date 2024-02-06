using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luma.Entities
{
    public class ProbeSyncData
    {
        public string ProbeName { get; set; }
        public long TimeOffset { get; set; }
        public long LastTimeOffset { get; set; }
        public long RoundTripTime { get; set; }
        //public long ProbeNow { get; set; }
        public string Encoding { get; set; }
    }
}
