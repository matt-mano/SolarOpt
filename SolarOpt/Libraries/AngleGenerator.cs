using SolarOpt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarOpt.Libraries
{
    public interface IAngleGenerator
    {
        //Get Angles for Today Will get the angles for today
        List<ServoAngle> getAnglesForToday(double latitude, double longitude);
    }
}
