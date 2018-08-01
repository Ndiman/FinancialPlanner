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

        //public ActionResult BudgetChart(int budgetId)
        //{
        //    return
        //}

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

        [Authorize(Roles = "HOH, Member")]
        public ActionResult MyDashboard(int houseId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var myDashboardData = new MyDashboardVM();
            var myBudgets = db.Budgets.Where(t => t.HouseholdId == user.HouseholdId);
            var myBudgetsId = db.Budgets.Where(t => t.HouseholdId == user.HouseholdId).FirstOrDefault();
            var myBudgetTransactions = db.Transactions.Where(t => t.BudgetId == myBudgetsId.Id);
            //var banks = db.Banks.Where(b => b.HouseholdId == user.HouseholdId).ToArray();
            //var accounts = db.Accounts.SelectMany(a => a.BankId == banks.)

            //Budget info
            myDashboardData.MyBudgets = myBudgets.OrderByDescending(b => b.SpendingTarget).ToList();
            myDashboardData.BudgetCnt = myBudgets.Count();

            myDashboardData.BankCnt = db.Banks.Where(b => b.HouseholdId == user.HouseholdId).Count();

            //myDashboardData.BankAccountCnt = 
            //var myAccountId = "";
            //for (var loop = 1; loop <= myDashboardData.BankCnt; loop++)
            //{
                
            //}

            //Transaction info
            //var myBankId = db.Banks.Where()
            //var myTransactions = db.Transactions.Where(t => t.AccountId)

            return View(myDashboardData);
        }

    }
}