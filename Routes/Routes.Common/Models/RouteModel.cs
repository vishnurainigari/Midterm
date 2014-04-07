using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutesCommon.Models
{
    public class RouteModel
    {
        public int RouteID { get; set; }

        public string RouteName { get; set; }

        public string RouteDescription { get; set; }

        public int FromID { get; set; }

        public string From { get; set; }


        public int ToID { get; set; }

        public string To { get; set; }
    }
}
