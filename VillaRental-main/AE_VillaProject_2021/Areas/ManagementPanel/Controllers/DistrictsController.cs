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
    public class DistrictsController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Districts
        public ActionResult Index()
        {
            var district = db.District.Include(d => d.Towns);
            return View(district.ToList());
        }

        // GET: ManagementPanel/Districts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // GET: ManagementPanel/Districts/Create
        public ActionResult Create()
        {
            ViewBag.TownId = new SelectList(db.Towns, "TowndId", "Name");
            return View();
        }

        // POST: ManagementPanel/Districts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DistrictId,Name,IsActive,RegisterDate,TownId")] District district)
        {
            if (ModelState.IsValid)
            {
                district.RegisterDate = DateTime.Now;
                db.District.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TownId = new SelectList(db.Towns, "TowndId", "Name", district.TownId);
            return View(district);
        }

        // GET: ManagementPanel/Districts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            ViewBag.TownId = new SelectList(db.Towns, "TowndId", "Name", district.TownId);
            return View(district);
        }

        // POST: ManagementPanel/Districts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DistrictId,Name,IsActive,RegisterDate,TownId")] District district)
        {
            if (ModelState.IsValid)
            {
                District update = db.District.Find(district.DistrictId);
                update.Name = district.Name;
                update.IsActive = district.IsActive;
                update.RegisterDate = DateTime.Now;
                update.TownId = district.TownId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TownId = new SelectList(db.Towns, "TowndId", "Name", district.TownId);
            return View(district);
        }

        // GET: ManagementPanel/Districts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // POST: ManagementPanel/Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            District district = db.District.Find(id);
            List<Products> villalar = db.Products.Where(x => x.ProductAddresses.DistrictId == district.DistrictId).ToList();
            foreach (var item in villalar)
            {
                foreach (var item2 in item.ProductImages.ToList())
                {
                    ImageUpload.DeleteByPath(item2.ImageUrl);
                    db.ProductImages.Remove(item2);
                }
                foreach (var item2 in item.FavoriProducts.ToList())
                {
                    db.FavoriProducts.Remove(item2);
                }
                foreach (var item2 in item.Sales.ToList())
                {
                    db.Sales.Remove(item2);
                }

                db.ProductDetails.Remove(item.ProductDetails);
                db.ProductAddresses.Remove(item.ProductAddresses);
                db.ProductProperties.Remove(item.ProductProperties);
                db.ProductNearPlace.Remove(item.ProductNearPlace);
                item.Categories.Clear();
                db.Products.Remove(item);
            }
            foreach (var item in district.Province.ToList())
            {
                db.Province.Remove(item);
            }
            db.District.Remove(district);
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
