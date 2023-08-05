using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIChefRemsey.Shared
{
    public class ReceptAfbeelding
    {
        public int created { get; set; }
        public ImageData[] data { get; set; }
    }

    public class ImageData
    {
        public string url { get; set; }
    }
}
