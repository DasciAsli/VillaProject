using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AE_VillaProject_2021.Models.EntityFramework;

namespace AE_VillaProject_2021.Areas.ManagementPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TermsController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Terms
        public ActionResult Index()
        {
            return View(db.Terms.ToList());
        }

        // GET: ManagementPanel/Terms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View(terms);
        }

        // GET: ManagementPanel/Terms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementPanel/Terms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TermsId,Title,Description,IsActive,RegisterDate")] Terms terms)
        {
            terms.RegisterDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Terms.Add(terms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(terms);
        }

        // GET: ManagementPanel/Terms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View(terms);
        }

        // POST: ManagementPanel/Terms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TermsId,Title,Description,IsActive,RegisterDate")] Terms terms)
        {
            
            if (ModelState.IsValid)
            {
                Terms update = db.Terms.Find(terms.TermsId);
                update.Title = terms.Title;
                update.Description = terms.Description;
                update.IsActive = terms.IsActive;
                update.RegisterDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(terms);
        }

        // GET: ManagementPanel/Terms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terms terms = db.Terms.Find(id);
            if (terms == null)
            {
                return HttpNotFound();
            }
            return View(terms);
        }

        // POST: ManagementPanel/Terms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Terms terms = db.Terms.Find(id);
            db.Terms.Remove(terms);
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
