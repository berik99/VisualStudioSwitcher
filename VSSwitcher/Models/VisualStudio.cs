using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestVSSelector.Models
{
    public class VisualStudio
    {
        public string VSID { get; set; }
        public VSVersion VSVersion { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
    }
}
