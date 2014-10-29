using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DapperExtensions;
using NetArgot.Models;

namespace NetArgot.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var car = GetCarByName("Volvo");
            return View();
        }

        public Car GetCarByName(string name)
        {
            using (DbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var predicate = Predicates.Field<Car>(f => f.Name, Operator.Like, name);
                return connection.Get<Car>(predicate);
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