using Routes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RoutesBL;
using RoutesCommon.Models;

namespace Routes.Controllers
{
    public class RouteController : Controller
    {
        //
        // GET: /Route/
        private RouteService _routeService = new RouteService();

        public ActionResult CreateRoute()
        {
            var pointsOfInterest = _routeService.GetPointsOfInterest();
            var poiItemsSource = new List<SelectListItem>();

            poiItemsSource.Add(new SelectListItem { Text = "-- Select Starting Location --", Value = "0" });
            foreach (var item in pointsOfInterest)
            {
                poiItemsSource.Add(new SelectListItem { Text = item.PointsOfInterestName, Value = item.PointsOfInterestID.ToString() });
            }

            var poiItemsSDestination = new List<SelectListItem>();

            poiItemsSDestination.Add(new SelectListItem { Text = "-- Select Destination Location --", Value = "0" });
            foreach (var item in pointsOfInterest)
            {
                poiItemsSDestination.Add(new SelectListItem { Text = item.PointsOfInterestName, Value = item.PointsOfInterestID.ToString() });
            }

            ViewData["poi_src_select"] = poiItemsSource;
            ViewData["poi_dest_select"] = poiItemsSDestination;

            return View();
        }

        public ActionResult RouteShop()
        {
            var routesTmp = _routeService.GetRoutes();
            var routes = new List<RouteViewModel>();

            foreach (var item in routesTmp)
            {
                routes.Add(GetRouteWithCheckPoints(item.RouteID));
            }

            ViewData["Routes"] = routes;

            return View();
        }

        [HttpPost]
        public JsonResult SaveRoute(string data)
        {
            RouteViewModel input = Newtonsoft.Json.JsonConvert.DeserializeObject<RouteViewModel>(data);
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(input.RouteName))
            {
                errors.Add("Route name is required");
            }
            if (input.FromID <= 0)
            {
                errors.Add("Starting location is required");
            }
            if (input.ToID <= 0)
            {
                errors.Add("Destination location is required");
            }

            if (input.FromID == input.ToID)
            {
                errors.Add("Starting and destination location should be different");
            }

            if (input.CheckPoints != null)
            {
                foreach (var item in input.CheckPoints)
	            {
                    if (string.IsNullOrWhiteSpace(item.CheckPointName))
	                {
                        errors.Add("All check points should have a name");
	                }
	            }
            }

            if (errors.Count > 0)
            {
                return Json(new
                {
                    success = false,
                    errors = errors
                });
            }

            var model = new RouteModel();
            model.RouteName = input.RouteName;
            model.RouteDescription = input.RouteDesc;
            model.FromID = input.FromID;
            model.ToID = input.ToID;

            List<CheckPointsModel> checkPoints = new List<CheckPointsModel>();
            foreach (var item in input.CheckPoints)
            {
                checkPoints.Add(new CheckPointsModel() { CheckPointName = item.CheckPointName, CheckPointDesc = item.CheckPointDesc });
            }

            _routeService.SaveRoute(model, checkPoints);

            return Json(new
            {
                success = true
            });
        }

        public ActionResult GetRouteDirections(int routeID)
        {
            var route = GetRouteWithCheckPoints(routeID);

            ViewData["Route"] = route;
            return View();
        }

        [HttpGet]
        public JsonResult GetRouteInfo(int routeID)
        {
            var route = GetRouteWithCheckPoints(routeID);

            return Json(new
            {
                data = route
            }, JsonRequestBehavior.AllowGet);
        }

        private RouteViewModel GetRouteWithCheckPoints(int routeID)
        {
            var route = _routeService.GetRoutes().FirstOrDefault(r => r.RouteID == routeID);

            if (route != null)
            {
                var checkPoints = _routeService.GetCheckPoints(routeID);

                var checkPointsViewModel = new List<CheckPointViewModel>();
                foreach (var item in checkPoints)
                {
                    var cp = new CheckPointViewModel();
                    cp.CheckPointName = item.CheckPointName;
                    cp.CheckPointDesc = item.CheckPointDesc;
                    checkPointsViewModel.Add(cp);
                }

                var pointsOfInterest = _routeService.GetPointsOfInterest();
                var from = pointsOfInterest.First(p => p.PointsOfInterestID == route.FromID).PointsOfInterestName;
                var to = pointsOfInterest.First(p => p.PointsOfInterestID == route.ToID).PointsOfInterestName;

                RouteViewModel viewModel = new RouteViewModel();
                viewModel.RouteID = route.RouteID;
                viewModel.RouteName = route.RouteName;
                viewModel.RouteDesc = route.RouteDescription;
                viewModel.CheckPoints = checkPointsViewModel;
                viewModel.From = from;
                viewModel.To = to;

                return viewModel;
            }

            return null;
        }
    }
}
