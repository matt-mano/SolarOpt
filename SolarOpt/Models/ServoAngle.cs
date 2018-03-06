using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarOpt.Models
{
    public class ServoAngle
    {
        public DateTime Timestamp { get; set; }
        public float Angle { get; set; }
        public long UnitId { get; set; }


    }
}
