using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Helpers;
using FinancialPlanner.Models;
using FinancialPlanner.View_Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlanner.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        public ActionResult Dashboard(int houseId)
        {
            return View();
        }

        public ActionResult ViewMembers(int houseId)
        {
            ViewBag.HouseId = houseId;
            return View(db.Users.Where(b => b.HouseholdId == houseId).ToList());
        }

        public ActionResult MyBudgets(int houseId)
        {
            return View(db.Budgets.Where(b => b.HouseholdId == houseId).ToList());
        }

        public ActionResult MyAccounts(int bankId)
        {
            return View(db.Accounts.Where(b => b.BankId == bankId).ToList());
        }

        public ActionResult MyTransactions(int accountId)
        {
            return View(db.Transactions.Where(b => b.AccountId == accountId).ToList());
        }

        public ActionResult MyBudgetItems(int budgetId)
        {
            return View(db.BudgetItems.Where(b => b.BudgetId == budgetId).ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Deleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var isGuest = roleHelper.IsUserInRole(userId, "Guest");
                if (isGuest == true)
                {
                    roleHelper.RemoveUserFromRole(userId, "Guest");
                }
                roleHelper.AddUserToRole(userId, "HOH");

                db.Households.Add(household);
                db.SaveChanges();

                var user = db.Users.Find(userId);
                user.HouseholdId = household.Id;
                db.SaveChanges();

                return RedirectToAction("MyDashboard", "Dashboard", new { houseId = user.HouseholdId});
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Deleted")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        [Authorize]
        public ActionResult Leave(int? houseId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            if (userRole == "HOH")
            {
                var memberCnt = db.Users.AsNoTracking().Where(u => u.HouseholdId == houseId).Count();
                if(memberCnt == 1)
                {
                    roleHelper.RemoveUserFromRole(userId, "HOH");
                    roleHelper.AddUserToRole(userId, "Guest");

                    user.Household = null;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    var houseHold = db.Households.Find(houseId);
                    houseHold.Deleted = true;
                    db.Entry(houseHold).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("newHOH", "Households");
                    
                }
            }
            else
            {
                roleHelper.RemoveUserFromRole(userId, "Member");
                roleHelper.AddUserToRole(userId, "Guest");

                user.HouseholdId = null;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        //GET
        public ActionResult newHOH(int? houseId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            var role = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var memberCnt = db.Users.AsNoTracking().Where(u => u.HouseholdId == houseId).Count();

            var data = new newHOHVM();
            {
                data.MemberList = new List<ApplicationUser>();
                var occupants = db.Users.Where(u => u.HouseholdId == houseId).ToList();
                foreach (var person in occupants)
                {
                    if (role == "Member")
                    {
                        data.MemberList.Add(person);
                    }
                }
            }
            
            
            ViewBag.Members = new SelectList(data.MemberList, "Id", "FirstName");

            if (role == "HOH" && memberCnt > 1)
            {
                return View(data);
            }

            return RedirectToAction("Index", "Home");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newHOH(newHOHVM newHOHVM)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            var role = roleHelper.ListUserRoles(userId).FirstOrDefault();
            if ( role == "HOH")
            {
                roleHelper.RemoveUserFromRole(userId, "HOH");
                roleHelper.AddUserToRole(userId, "Guest");

                user.Household = null;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            else //if selected
            {
                roleHelper.RemoveUserFromRole(userId, "Member");
                roleHelper.AddUserToRole(userId, "HOH");
            }
            return RedirectToAction("Index", "Home");
        }

        //// GET: Households/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Household household = db.Households.Find(id);
        //    if (household == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(household);
        //}

        //// POST: Households/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Household household = db.Households.Find(id);
        //    db.Households.Remove(household);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
