using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using DapperExtensions;
using NetArgot.Models;

namespace NetArgot.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var car = GetCarByName("Volvo");
            return View();
        }

        public Car GetCarByName(string name)
        {
            using (DbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                IList<Car> cars = connection.GetList<Car>(Predicates.Field<Car>(f => f.Name, Operator.Eq, name)).ToList();
                //Car car = connection.Query<Car>("SELECT * from Car WHERE Name = @name",new{ name });

                return cars.FirstOrDefault();
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}