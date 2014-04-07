using RoutesCommon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutesBL
{
    public class RouteService
    {
        public List<RouteModel> GetRoutes()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            List<RouteModel> result = new List<RouteModel>();
            List<PointsOfInterestModel> pois = GetPointsOfInterest();
 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT RouteID, RouteName, RouteDesc, From_PointOfInterestID, To_PointOfInterestID FROM dbo.Route;",
                    connection);
                    
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RouteModel model = new RouteModel();
                        model.RouteID = Convert.ToInt32(reader["RouteID"]);
                        model.RouteName = reader["RouteName"].ToString();
                        model.RouteDescription = reader["RouteDesc"].ToString();
                        model.FromID = (int)reader["From_PointOfInterestID"];
                        model.From = pois.First(p => p.PointsOfInterestID == (int)reader["From_PointOfInterestID"]).PointsOfInterestName;
                        model.ToID = (int)reader["To_PointOfInterestID"];
                        model.To = pois.First(p => p.PointsOfInterestID == (int)reader["From_PointOfInterestID"]).PointsOfInterestName;
                        
                        result.Add(model);
                    }
                }
                reader.Close();
            }

            return result;
        }

        public List<PointsOfInterestModel> GetPointsOfInterest()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            List<PointsOfInterestModel> result = new List<PointsOfInterestModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "SELECT PointsOfInterestID, PointsOfInterestName FROM dbo.PointsOfInterest; ",
                    connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        PointsOfInterestModel model = new PointsOfInterestModel();
                        model.PointsOfInterestID = Convert.ToInt32(reader["PointsOfInterestID"]);
                        model.PointsOfInterestName = reader["PointsOfInterestName"].ToString();

                        result.Add(model);
                    }
                }
                reader.Close();
            }

            return result;
        }

        public List<CheckPointsModel> GetCheckPoints(int routeID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            List<CheckPointsModel> result = new List<CheckPointsModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(
                    string.Format("SELECT CheckPointID, CheckPointName, CheckPointDesc FROM dbo.CheckPoints WHERE routeid = {0};", routeID),
                    connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CheckPointsModel model = new CheckPointsModel();
                        model.CheckPointID = Convert.ToInt32(reader["CheckPointID"]);
                        model.CheckPointName = reader["CheckPointName"].ToString();
                        model.CheckPointDesc = reader["CheckPointDesc"].ToString();
                        model.RouteID = routeID;

                        result.Add(model);
                    }
                }
                reader.Close();
            }

            return result;
        }

        public bool SaveRoute(RouteModel model, List<CheckPointsModel> checkPoints)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                string query = "INSERT INTO dbo.Route (RouteName, RouteDesc, From_PointOfInterestID, To_PointOfInterestID) " +
                       "VALUES (@RouteName, @RouteDesc, @From_PointOfInterestID, @To_PointOfInterestID) ";

                using (SqlConnection cn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@RouteName", SqlDbType.VarChar, 50).Value = model.RouteName;
                    cmd.Parameters.Add("@RouteDesc", SqlDbType.VarChar, 1000).Value = model.RouteDescription;
                    cmd.Parameters.Add("@From_PointOfInterestID", SqlDbType.Int).Value = model.FromID;
                    cmd.Parameters.Add("@To_PointOfInterestID", SqlDbType.Int).Value = model.ToID;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }

                var routes = GetRoutes();
                if (routes != null)
                {
                    var routeID = routes.OrderByDescending(r => r.RouteID).First().RouteID;

                    foreach (var item in checkPoints)
                    {
                        item.RouteID = routeID;
                        SaveCheckPoint(item);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SaveCheckPoint(CheckPointsModel model)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                string query = "INSERT INTO dbo.CheckPoints (CheckPointName, CheckPointDesc, RouteID) " +
                       "VALUES (@CheckPointName, @CheckPointDesc, @RouteID) ";

                using (SqlConnection cn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.Add("@CheckPointName", SqlDbType.VarChar, 50).Value = model.CheckPointName;
                    cmd.Parameters.Add("@CheckPointDesc", SqlDbType.VarChar, 1000).Value = model.CheckPointDesc;
                    cmd.Parameters.Add("@RouteID", SqlDbType.Int).Value = model.RouteID;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
