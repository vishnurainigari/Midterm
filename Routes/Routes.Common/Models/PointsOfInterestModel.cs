using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutesCommon.Models
{
    public class PointsOfInterestModel
    {
        public int PointsOfInterestID { get; set; }

        public string PointsOfInterestName { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
    }
}
