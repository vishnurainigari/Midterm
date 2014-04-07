using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Routes.Models
{
    public class RouteViewModel
    {
        public int RouteID { get; set; }

        public string RouteName { get; set; }

        public string RouteDesc { get; set; }

        public int FromID { get; set; }

        public string From { get; set; }

        public int ToID { get; set; }

        public string To { get; set; }

        public List<CheckPointViewModel> CheckPoints { get; set; }
    }

    public class CheckPointViewModel
    {
        public string CheckPointName { get; set; }

        public string CheckPointDesc { get; set; }
    }
}