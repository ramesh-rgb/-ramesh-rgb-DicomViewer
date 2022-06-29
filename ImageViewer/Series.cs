using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public  class Series
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<string> files { get; set; } = new List<string>();
    }
}
