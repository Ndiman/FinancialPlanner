using FinancialPlanner.Models;
using FinancialPlanner.View_Models;
using Microsoft.AspNet.Identity;
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
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (User.IsInRole("HOH") || User.IsInRole("Member"))
                return RedirectToAction("MyDashboard", new { houseId = user.HouseholdId });
            else
                return View();
        }

        public ActionResult MyDashboard(int houseId)
        {
            var myDashboardData = new MyDashboardVM();

            return View();
        }

    }
}