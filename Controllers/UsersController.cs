using FinancialPlanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FinancialPlanner.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var applicationUsers = db.Users.Include(a => a.Household);
            return View(applicationUsers.ToList());
        }

        //GET: Users/Details
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        //GET: Users/Edit
        public ActionResult Edit (string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        //POST: Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DisplayName,AvatarPath,Email")] ApplicationUser applicationUser, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    var width = image.Width;
                    var height = image.Height;
                    if (width > height)
                    {
                        var leftRightCrop = (width - height) / 2;
                        image.Crop(0, leftRightCrop, 0, leftRightCrop);
                    }
                    else if (height > width)
                    {
                        var topBottomCrop = (height - width) / 2;
                        image.Crop(topBottomCrop, 0, topBottomCrop, 0);
                    }

                    var filename = Path.GetFileName(image.FileName).Replace(' ', '_');
                    image.Save(Path.Combine(Server.MapPath("../Avatars/"), filename));
                    filename = Path.Combine("~/Avatars/" + filename);

                    applicationUser.AvatarPath = Url.Content(filename);
                }
                
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(applicationUser);
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}