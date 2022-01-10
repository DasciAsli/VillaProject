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
    public class SalesController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Sales
        public ActionResult Index()
        {
            var sales = db.Sales.Include(s => s.Products).Include(s => s.Users);
            return View(sales.ToList());
        }

        // GET: ManagementPanel/Sales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            return View(sales);
        }

        // GET: ManagementPanel/Sales/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Title");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name");
            return View();
        }

        // POST: ManagementPanel/Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SalesId,UserId,ProductId,EntryDate,ReleaseDate,RegisterDate,IsActive,TotalPrice")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                db.Sales.Add(sales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Title", sales.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", sales.UserId);
            return View(sales);
        }

        // GET: ManagementPanel/Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Title", sales.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", sales.UserId);
            return View(sales);
        }

        // POST: ManagementPanel/Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SalesId,UserId,ProductId,EntryDate,ReleaseDate,RegisterDate,IsActive,TotalPrice")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Title", sales.ProductId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", sales.UserId);
            return View(sales);
        }

        // GET: ManagementPanel/Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sales sales = db.Sales.Find(id);
            if (sales == null)
            {
                return HttpNotFound();
            }
            return View(sales);
        }

        // POST: ManagementPanel/Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sales sales = db.Sales.Find(id);
            db.Sales.Remove(sales);
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
