using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luma.Entities
{
    internal class ProbesClass
    {
        public ProbeResponse[] Probes { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
    internal class ProbeResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Encoding { get; set; }
    }
}
