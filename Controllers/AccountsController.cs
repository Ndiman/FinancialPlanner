using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;

namespace FinancialPlanner.Controllers
{
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index(int bankId)
        {
            //ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Name");
            //var accounts = db.Accounts.Include(a => a.Bank).Include(a => a.Type);
            var BankName = db.Banks.Find(bankId);
            ViewBag.Bank = BankName.Name;
            var houseId = BankName.HouseholdId;
            ViewBag.HouseId = houseId;
            ViewBag.BankId = bankId;
            return View(db.Accounts.Where(a => a.BankId == bankId).ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bank = account.Bank.Name;
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create(int bankId)
        {
            var BankName = db.Banks.Find(bankId);
            ViewBag.Bank = BankName.Name;
            ViewBag.HouseId = BankName.HouseholdId;
            ViewBag.BankId = bankId;
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountTypeId,BankId,Name,Description,StartingBalance")] Account account, int bankId)
        {
            if (ModelState.IsValid)
            {
                account.CurrentBalance = account.StartingBalance;
                account.Created = DateTimeOffset.Now;

                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index", new { bankId});
            }

            ViewBag.Bank = account.Bank.Name;
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name", account.BankId);
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type", account.AccountTypeId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bank = account.Bank.Name;
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "Id", "Type", account.AccountTypeId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountTypeId,BankId,Name,Description,Created,Updated,StartingBalance,CurrentBalance")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.Updated = DateTimeOffset.Now;

                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { bankId = account.BankId});
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name", account.BankId);
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type", account.AccountTypeId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bank = account.Bank.Name;
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index", new { bankId = account.BankId });
        }

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
