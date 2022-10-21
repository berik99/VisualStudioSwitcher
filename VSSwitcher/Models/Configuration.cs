using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestVSSelector.Models;

namespace VSSwitcher.Models
{
    public class Configuration
    {
        public Configuration()
        {
            VSList = new List<VisualStudio>();
            Solutions = new List<Solution>();
        }

        public List<VisualStudio> VSList { get; set; }
        public List<Solution> Solutions { get; set; }
    }
}
