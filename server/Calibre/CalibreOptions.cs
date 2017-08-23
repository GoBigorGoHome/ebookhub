using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebookhub.Calibre
{
    public class CalibreOptions
    {
        public string CalibreBinFolder { get; set; }
        public string ContentFolder { get; set; }
        public List<string> FileTypes { get; set; }
    }
}
