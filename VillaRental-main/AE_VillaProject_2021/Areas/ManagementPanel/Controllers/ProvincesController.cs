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
    public class ProvincesController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Provinces
        public ActionResult Index()
        {
            var province = db.Province.Include(p => p.District);
            return View(province.ToList());
        }

        // GET: ManagementPanel/Provinces/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Province province = db.Province.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            return View(province);
        }

        // GET: ManagementPanel/Provinces/Create
        public ActionResult Create()
        {
            ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name");
            return View();
        }

        // POST: ManagementPanel/Provinces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProvinceId,Name,IsActive,RegisterDate,DistrictId")] Province province)
        {
            if (ModelState.IsValid)
            {
                province.RegisterDate = DateTime.Now;
                db.Province.Add(province);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name", province.DistrictId);
            return View(province);
        }

        // GET: ManagementPanel/Provinces/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Province province = db.Province.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name", province.DistrictId);
            return View(province);
        }

        // POST: ManagementPanel/Provinces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProvinceId,Name,IsActive,RegisterDate,DistrictId")] Province province)
        {
            if (ModelState.IsValid)
            {
                Province update = db.Province.Find(province.ProvinceId);
                update.Name = province.Name;
                update.IsActive = province.IsActive;
                update.RegisterDate = DateTime.Now;
                update.DistrictId = province.DistrictId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name", province.DistrictId);
            return View(province);
        }

        // GET: ManagementPanel/Provinces/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Province province = db.Province.Find(id);
            if (province == null)
            {
                return HttpNotFound();
            }
            return View(province);
        }

        // POST: ManagementPanel/Provinces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Province province = db.Province.Find(id);
            List<Products> villalar = db.Products.Where(x => x.ProductAddresses.ProvinceId == province.ProvinceId).ToList();
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
           
            db.Province.Remove(province);
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
