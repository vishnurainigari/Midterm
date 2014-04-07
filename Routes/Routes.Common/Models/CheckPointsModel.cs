using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutesCommon.Models
{
    public class CheckPointsModel
    {
        public int CheckPointID { get; set; }

        public string CheckPointName { get; set; }

        public string CheckPointDesc { get; set; }

        public int RouteID { get; set; }
    }
}
