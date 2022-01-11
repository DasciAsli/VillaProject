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
    public class TownsController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Towns
        public ActionResult Index()
        {
            return View(db.Towns.ToList());
        }

        // GET: ManagementPanel/Towns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Towns towns = db.Towns.Find(id);
            if (towns == null)
            {
                return HttpNotFound();
            }
            return View(towns);
        }

        // GET: ManagementPanel/Towns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementPanel/Towns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TowndId,Name,RegisterDate,IsActive")] Towns towns)
        {
            if (ModelState.IsValid)
            {
                towns.RegisterDate = DateTime.Now;
                db.Towns.Add(towns);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(towns);
        }

        // GET: ManagementPanel/Towns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Towns towns = db.Towns.Find(id);
            if (towns == null)
            {
                return HttpNotFound();
            }
            return View(towns);
        }

        // POST: ManagementPanel/Towns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TowndId,Name,RegisterDate,IsActive")] Towns towns)
        {
            if (ModelState.IsValid)
            {
                Towns update = db.Towns.Find(towns.TowndId);
                update.Name = towns.Name;
                update.IsActive = towns.IsActive;
                if (update.IsActive==false)
                {
                    var ilceler = db.District.Where(x => x.TownId == towns.TowndId).ToList();
                    foreach (var item in ilceler)
                    {
                        item.IsActive = false;
                        foreach (var item2 in item.Province)
                        {
                            item2.IsActive = false;
                        }
                    }
                    var villalar = db.Products.Where(x => x.ProductAddresses.TownId == towns.TowndId).ToList();
                    foreach (var item in villalar)
                    {
                        item.IsActive = false;
                    }
                }
                if (update.IsActive == true)
                {
                    var ilceler = db.District.Where(x => x.TownId == towns.TowndId).ToList();
                    foreach (var item in ilceler)
                    {
                        item.IsActive = true;
                        foreach (var item2 in item.Province)
                        {
                            item2.IsActive = true;
                        }
                    }
                    var villalar = db.Products.Where(x => x.ProductAddresses.TownId == towns.TowndId).ToList();
                    foreach (var item in villalar)
                    {
                        item.IsActive = true;
                    }
                }

                update.RegisterDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(towns);
        }

        // GET: ManagementPanel/Towns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Towns towns = db.Towns.Find(id);
            if (towns == null)
            {
                return HttpNotFound();
            }
            return View(towns);
        }

        // POST: ManagementPanel/Towns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Towns towns = db.Towns.Find(id);
            List<Products> villalar = db.Products.Where(x => x.ProductAddresses.TownId == towns.TowndId).ToList();
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
            foreach (var item in towns.District.ToList())
            {
                foreach (var item2 in item.Province.ToList())
                {
                    db.Province.Remove(item2);
                }
            }
            foreach (var item in towns.District.ToList())
            {
                db.District.Remove(item);
             
            }
            foreach (var item in towns.District.ToList())
            {
                db.District.Remove(item);

            }

            db.Towns.Remove(towns);
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
