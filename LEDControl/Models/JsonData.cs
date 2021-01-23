using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LEDControl.Models
{
    public class JsonData
    {
        public JsonColor jsonColor { get; set; }

        public int WaitTime { get; set; }
        public bool Loop { get; set; }
        public int Iterations { get; set; } = 1;
        public int Length { get; set; }
        public int Duration { get; set; }
        public int Trail { get; set; }

    }

    public class JsonColor
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}
