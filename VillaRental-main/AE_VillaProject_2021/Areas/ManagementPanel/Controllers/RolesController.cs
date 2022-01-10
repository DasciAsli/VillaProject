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
    public class RolesController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Roles
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        }

        // GET: ManagementPanel/Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // GET: ManagementPanel/Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementPanel/Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoleId,RoleName,IsActive,RegisterDate")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                roles.RegisterDate = DateTime.Now;
                db.Roles.Add(roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roles);
        }

        // GET: ManagementPanel/Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // POST: ManagementPanel/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoleId,RoleName,IsActive,RegisterDate")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                Roles update = db.Roles.Find(roles.RoleId);
                update.RoleName = roles.RoleName;
                update.IsActive = roles.IsActive;
                update.RegisterDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roles);
        }

        // GET: ManagementPanel/Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // POST: ManagementPanel/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Roles roles = db.Roles.Find(id);
            foreach (var item in roles.Users.ToList())
            {
                foreach (var item2 in item.FavoriProducts.ToList())
                {
                    db.FavoriProducts.Remove(item2);
                }
                foreach (var item2 in item.Sales.ToList())
                {
                    db.Sales.Remove(item2);
                }
                if (item.UserDetails.ImageUrl != null)
                {
                    ImageUpload.DeleteByPath(item.UserDetails.ImageUrl);
                }
                db.UserDetails.Remove(item.UserDetails);
                db.Users.Remove(item);

            }
            db.Roles.Remove(roles);
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
