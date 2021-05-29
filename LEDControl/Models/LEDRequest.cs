using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEDControl.Models
{
    public class LEDRequest
    {
        public Mode Mode { get; set; }
        public LEDSettings Settings { get; set; }
    }
}
