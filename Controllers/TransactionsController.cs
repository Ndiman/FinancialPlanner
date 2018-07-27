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
using Microsoft.AspNet.Identity;

namespace FinancialPlanner.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(int accountId)
        {
            //var transactions = db.Transactions.Include(t => t.Account);
            var account = db.Accounts.Find(accountId);
            ViewBag.Account = account.Name;
            var bankId = account.BankId;
            var bankName = db.Banks.Find(bankId);
            ViewBag.Bank = bankName.Name;
            ViewBag.BankId = bankId;
            ViewBag.AccountId = accountId;
            return View(db.Transactions.Where(t => t.AccountId == accountId).ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            ViewBag.Account = transaction.Account.Name;
            var bankId = transaction.Account.BankId;
            var bankName = db.Banks.Find(bankId);
            ViewBag.Bank = bankName.Name;
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create(int accountId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myHouse = user.HouseholdId;
            var myBudgets = db.Budgets.Where(b => b.HouseholdId == myHouse).ToList();

            var account = db.Accounts.Find(accountId);
            ViewBag.Account = account.Name;
            var bankId = account.BankId;
            var bank = db.Banks.Find(bankId);
            ViewBag.Bank = bank.Name;
            ViewBag.BankId = bankId;
            ViewBag.AccountId = accountId;

            ViewBag.BudgetId = new SelectList(myBudgets, "Id", "Name");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,Amount,Title,Memo,Created,TransactionTypeId,BudgetId")] Transaction transaction)
        {
            var accountId = transaction.AccountId;
            var account = db.Accounts.Find(accountId);
            //var oldBalance = db.Accounts.AsNoTracking().FirstOrDefault(a => a.CurrentBalance == account.CurrentBalance);

            if (ModelState.IsValid)
            {
                transaction.Created = DateTimeOffset.Now;
                //transaction.Memo = name;

                db.Transactions.Add(transaction);
                db.SaveChanges();

                decimal num1 = account.CurrentBalance;
                decimal num2 = transaction.Amount;
                var transactionTypeId = transaction.TransactionTypeId;
                var transactionType = db.TransactionTypes.Find(transactionTypeId);
                if(transactionType.Name == "Debit")
                {
                    decimal result = num1 - num2;
                    account.CurrentBalance = result;
                }
                else if(transactionType.Name == "Credit")
                {
                    decimal result = num1 + num2;
                    account.CurrentBalance = result;
                }

                if(transaction.BudgetId != null)
                {
                    var budgetId = transaction.BudgetId;
                    var budget = db.Budgets.Find(budgetId);
                    decimal result = budget.CurrentBalance + transaction.Amount;
                    budget.CurrentBalance = result;
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = transaction.AccountId});
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.Account = transaction.Account.Name;
            var bankId = transaction.Account.BankId;
            var bankName = db.Banks.Find(bankId);
            ViewBag.Bank = bankName.Name;
            ViewBag.AccountId = transaction.AccountId;

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myHouse = user.HouseholdId;
            var myBudgets = db.Budgets.Where(b => b.HouseholdId == myHouse).ToList();
            if(transaction.BudgetId != null)
            {
                ViewBag.BudgetId = new SelectList(myBudgets, "Id", "Name", transaction.BudgetId);
            }
            else
            {
                ViewBag.BudgetId = new SelectList(myBudgets, "Id", "Name");
            }
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "Name", transaction.TransactionTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Amount,Title,Memo,Created,Updated,Reconciled,ReconciledAmount,TransactionTypeId,BudgetId")] Transaction transaction)
        {
            var oldTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
            //var oldTransaction = db.Transactions.Find(transactionId);
            var oldAmount = oldTransaction.Amount;
            var oldBudgetId = oldTransaction.BudgetId;

            if (ModelState.IsValid)
            {
                transaction.Updated = DateTimeOffset.Now;

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();

                var newAmount = transaction.Amount;
                var typeId = transaction.TransactionTypeId;
                var name = db.TransactionTypes.Find(typeId).Name;
                var accountId = transaction.AccountId;
                var account = db.Accounts.Find(accountId);
                if (transaction.ReconciledAmount > 0)
                {
                    transaction.Reconciled = true;
                }
                if (oldAmount != newAmount)
                {
                    if (oldAmount > newAmount)
                    {
                        var num = oldAmount - newAmount;

                        if (name == "Debit")
                        {
                            decimal result = account.CurrentBalance + num;
                            account.CurrentBalance = result;
                        }
                        else if(name == "Credit")
                        {
                            decimal result = account.CurrentBalance - num;
                            account.CurrentBalance = result;
                        }
                    }
                    else if(newAmount > oldAmount)
                    {
                        var num = newAmount - oldAmount;

                        if(name == "Debit")
                        {
                            decimal result = account.CurrentBalance - num;
                            account.CurrentBalance = result;
                        }
                        else if(name == "Credit")
                        {
                            decimal result = account.CurrentBalance + num;
                            account.CurrentBalance = result;
                        }
                    }
                }

                if(transaction.BudgetId != null && (oldBudgetId == transaction.BudgetId))
                {
                    var budgetId = transaction.BudgetId;
                    var budget = db.Budgets.Find(budgetId);
                    decimal result = budget.CurrentBalance + transaction.Amount - oldAmount;
                    budget.CurrentBalance = result;
                }
                else if(transaction.BudgetId != null && (oldBudgetId != transaction.BudgetId))
                {
                    var budgetId = transaction.BudgetId;
                    var budget = db.Budgets.Find(budgetId);
                    if(oldBudgetId != null)
                    {
                        var oldBudget = db.Budgets.Find(oldBudgetId);
                        decimal result1 = oldBudget.CurrentBalance - oldAmount;
                        oldBudget.CurrentBalance = result1;
                    }
                    decimal result2 = budget.CurrentBalance + transaction.Amount;
                    budget.CurrentBalance = result2;
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { accountId = transaction.AccountId});
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.Account = transaction.Account.Name;
            var bankId = transaction.Account.BankId;
            var bankName = db.Banks.Find(bankId);
            ViewBag.Bank = bankName.Name;
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);

            var accountId = transaction.AccountId;
            var account = db.Accounts.Find(accountId);
            decimal num1 = account.CurrentBalance;
            decimal num2 = transaction.Amount;
            var transactionTypeId = transaction.TransactionTypeId;
            var transactionType = db.TransactionTypes.Find(transactionTypeId);
            if (transactionType.Name == "Debit")
            {
                decimal result = num1 + num2;
                account.CurrentBalance = result;
            }
            else if (transactionType.Name == "Credit")
            {
                decimal result = num1 - num2;
                account.CurrentBalance = result;
            }

            if(transaction.BudgetId != null)
            {
                var budgetId = transaction.BudgetId;
                var budget = db.Budgets.Find(budgetId);
                decimal result = budget.CurrentBalance - transaction.Amount;
                budget.CurrentBalance = result;
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index", new { accountId = transaction.AccountId});
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
