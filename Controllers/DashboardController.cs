using FinancialPlanner.Models;
using FinancialPlanner.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinancialPlanner.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyDashboard(int houseId)
        {
            var myDashboardData = new MyDashboardVM();

            return View();
        }

    }
}