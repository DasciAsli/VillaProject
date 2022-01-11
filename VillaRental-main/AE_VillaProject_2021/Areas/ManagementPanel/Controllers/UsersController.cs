using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AE_VillaProject_2021.Models.EntityFramework;
using PA_BlogProject_2021.Areas.ManagementPanel.Helpers;

namespace AE_VillaProject_2021.Areas.ManagementPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Roles).Include(u => u.UserDetails);
            return View(users.ToList());
        }
        public ActionResult Search(string p)
        {
            IEnumerable<Users> users = db.Users.Where(x => x.Name == p || x.SurName == p || (x.Name + " " + x.SurName) == p).ToList();
            if (users.Count() == 0)
            {
                users = db.Users.ToList();
            }
            return View("Index",users);
        }

        // GET: ManagementPanel/Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: ManagementPanel/Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName");
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "Age");
            return View();
        }

        // POST: ManagementPanel/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "UserId,Name,SurName,Email,Password,IsActive,RegisterDate,RoleId,UserDetails")] Users users, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    users.UserDetails.ImageUrl = ImageUpload.SaveImage(imgFile, 80, 83);
                }
                var hash = users.Password.GetHashCode().ToString();
                users.Password = hash;
                users.RegisterDate = DateTime.Now;
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", users.RoleId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "Age", users.UserId);
            return View(users);
        }

        // GET: ManagementPanel/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", users.RoleId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "Age", users.UserId);
            return View(users);
        }

        // POST: ManagementPanel/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "UserId,Name,SurName,Email,Password,IsActive,RegisterDate,RoleId,UserDetails")] Users users,HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                Users update = db.Users.Find(users.UserId);
                if (update.Password == users.Password)
                {
                    update.Password = users.Password;
                }
                else
                {
                    var hash = users.Password.GetHashCode().ToString();
                    update.Password = hash;
                }
                if (imgFile != null)
                {
                    ImageUpload.DeleteByPath(update.UserDetails.ImageUrl);
                    update.UserDetails.ImageUrl = ImageUpload.SaveImage(imgFile, 80, 83);
                }
                update.Name = users.Name;
                update.SurName = users.SurName;
                update.Email = users.Email;
                update.IsActive = users.IsActive;
                update.RegisterDate = DateTime.Now;
                update.RoleId = users.RoleId;
                update.UserDetails.Gender = users.UserDetails.Gender;
                update.UserDetails.Age = users.UserDetails.Age;
                update.UserDetails.PhoneNumber = users.UserDetails.PhoneNumber;
                update.UserDetails.Address = users.UserDetails.Address;
                update.UserDetails.Tc = users.UserDetails.Tc;
                update.UserDetails.PassportNumber = users.UserDetails.PassportNumber;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "RoleId", "RoleName", users.RoleId);
            ViewBag.UserId = new SelectList(db.UserDetails, "UserId", "Age", users.UserId);
            return View(users);
        }

        // GET: ManagementPanel/Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: ManagementPanel/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            UserDetails details = db.UserDetails.Find(id);
            foreach (var item in users.FavoriProducts.ToList())
            {
                db.FavoriProducts.Remove(item);
            }
            foreach (var item in users.Sales.ToList())
            {
                db.Sales.Remove(item);
            }
            if (details.ImageUrl != null)
            {
                ImageUpload.DeleteByPath(details.ImageUrl);
            }          
            db.UserDetails.Remove(details);
            db.Users.Remove(users);
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
