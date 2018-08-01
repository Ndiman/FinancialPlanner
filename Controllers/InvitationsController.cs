using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Helpers;
using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlanner.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Invitations
        public ActionResult Index()
        {
            return View(db.Invitations.ToList());
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        [Authorize(Roles = "HOH")]
        // GET: Invitations/Create
        public ActionResult Create(int houseId)
        {
            ViewBag.HouseholdId = houseId;
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "HouseholdId,Email,Body,LifeSpan")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                invitation.SenderId = User.Identity.GetUserId();
                invitation.LifeSpan = 30;
                invitation.Created = DateTime.Now;
                invitation.Code = Guid.NewGuid().ToString();

                db.Invitations.Add(invitation);
                db.SaveChanges();

                //email
                var callbackUrl = Url.Action("Register", "Account", new { email = invitation.Email, code = invitation.Code }, protocol: Request.Url.Scheme);
                var houseName = db.Households.Find(invitation.HouseholdId).Name;
                var senderFName = db.Users.Find(invitation.SenderId).FirstName;
                var senderLName = db.Users.Find(invitation.SenderId).LastName;

                var email = new IdentityMessage()
                {
                    Subject = string.Format(senderFName + " " + senderLName + " " + "has invited you to join their Household on Dandelion | Financial Planner"),
                    Body = invitation.Body + "<br /> Click <a href=\"" + callbackUrl + "\">this link</a> to accept the invitation",
                    Destination = invitation.Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);

                return RedirectToAction("MyDashboard", "Dashboard");
            }
            else
            {
                ViewBag.HouseholdId = invitation.HouseholdId;

                var message = string.Join(" | ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));

                ModelState.AddModelError("", message);
                return View(invitation);
            }

        }

       

       
        public ActionResult Join(string email, string code)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var invite = db.Invitations.FirstOrDefault(i => i.Email == email && i.Code == code);
            if (invite != null)
            {
                invite.Accepted = true;
                db.SaveChanges();
                
                user.HouseholdId = invite.HouseholdId;
                db.SaveChanges();

                roleHelper.RemoveUserFromRole(userId, "Guest");
                roleHelper.AddUserToRole(userId, "Member");
            }
            return RedirectToAction("MyDashboard", "Dashboard", new { houseId = user.HouseholdId});
        }

        public ActionResult SearchGuests(string searchString)
        {
            var users = db.Users.Where(u => u.HouseholdId == null).ToList();
            return View(users);
        }

        [Authorize(Roles = "HOH")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUser([Bind(Include = "HouseholdId,Email,LifeSpan")] Invitation invitation, int houseId, string Email)
        {
            if (ModelState.IsValid)
            {
                invitation.SenderId = User.Identity.GetUserId();
                invitation.LifeSpan = 30;
                invitation.Created = DateTime.Now;
                invitation.Code = Guid.NewGuid().ToString();
                invitation.HouseholdId = houseId;

                db.Invitations.Add(invitation);
                db.SaveChanges();

                //email
                var callbackUrl = Url.Action("Login", "Account", new { email = invitation.Email, code = invitation.Code }, protocol: Request.Url.Scheme);
                var houseName = db.Households.Find(invitation.HouseholdId).Name;
                var senderFName = db.Users.Find(invitation.SenderId).FirstName;
                var senderLName = db.Users.Find(invitation.SenderId).LastName;

                var email = new IdentityMessage()
                {
                    Subject = "Dandelion | Financial Planner Household Invitation",
                    Body = string.Format(senderFName + " " + senderLName + " " + "has invited you to join their Household on Dandelion | Financial Planner") + "<br /> Click <a href=\"" + callbackUrl + "\">this link</a> to accept the invitation",
                    Destination = invitation.Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);

                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                TempData["sweetMsg"] = "An invitation has been sent";

                return RedirectToAction("MyDashboard", "Dashboard", new { houseId = user.HouseholdId});
            }

            return View();

        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Created,Email,Body,Code,LifeSpan,Accepted")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
