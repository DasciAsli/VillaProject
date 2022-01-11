using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AE_VillaProject_2021.Models;
using AE_VillaProject_2021.Models.EntityFramework;
using PA_BlogProject_2021.Areas.ManagementPanel.Helpers;
using PagedList;
using PagedList.Mvc;

namespace AE_VillaProject_2021.Areas.ManagementPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private VillaContext db = new VillaContext();
        TownDistrictProvince tdp = new TownDistrictProvince();

        // GET: ManagementPanel/Products
        public ActionResult Index(int sayfa=1)
        {
            var products = db.Products.Include(p => p.ProductAddresses).Include(p => p.ProductDetails).Include(p => p.ProductNearPlace).Include(p => p.ProductProperties).Include(p=>p.ProductImages).OrderBy(x => x.ProductId).ToPagedList(sayfa, 2);
            ViewBag.PageList = db.Products.Include(p => p.ProductAddresses).Include(p => p.ProductDetails).Include(p => p.ProductNearPlace).Include(p => p.ProductProperties).Include(p => p.ProductImages).OrderBy(x => x.ProductId).ToPagedList(sayfa, 2);
            return View(products.ToList());
        }

        // GET: ManagementPanel/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: ManagementPanel/Products/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.ProductAddresses, "ProductId", "Adress");
            ViewBag.ProductId = new SelectList(db.ProductDetails, "ProductId", "RoomCount");
            ViewBag.ProductId = new SelectList(db.ProductNearPlace, "ProductId", "Education");
            ViewBag.ProductId = new SelectList(db.ProductProperties, "ProductId", "ProductId");
            ViewBag.CategoryId = db.Categories.ToList();
            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Name");
            //ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name");
            //ViewBag.TowndId = new SelectList(db.Towns, "TowndId", "Name");
            tdp.Sehirler = new SelectList(db.Towns, "TowndId", "Name");
            tdp.İlceler = new SelectList(db.District, "DistrictId", "Name");
            tdp.Bölgeler = new SelectList(db.Province, "ProvinceId", "Name");
            ViewBag.tdp = tdp;
            return View();
        }
        public JsonResult ilcegetir(int p)
        {
            var ilceler = (from x in db.District
                           join y in db.Towns on x.Towns.TowndId equals y.TowndId
                           where x.Towns.TowndId == p
                           select new
                           {
                               Text = x.Name,
                               Value = x.DistrictId.ToString()
                           }).ToList();
            return Json(ilceler,JsonRequestBehavior.AllowGet);
        }
        public JsonResult bölgegetir(int p)
        {
            var bölgeler = (from x in db.Province
                           join y in db.District on x.District.DistrictId equals y.DistrictId
                           where x.District.DistrictId == p
                           select new
                           {
                               Text = x.Name,
                               Value = x.ProvinceId.ToString()
                           }).ToList();
            return Json(bölgeler, JsonRequestBehavior.AllowGet);
        }

        // POST: ManagementPanel/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]

        public ActionResult Create([Bind(Include = "ProductId,Title,SubTitle,Description,Price,Metrekare,IsActive,RegisterDate,Deposit,ProductAddresses,ProductDetails,ProductNearPlace,ProductProperties")] Products products, Province province, District district, Towns towns, List<int> CategoryId, List<HttpPostedFileBase> imgFiles)
        {
            if (ModelState.IsValid)
            {
                if (CategoryId.Count() != 0)
                {
                    foreach (var item in CategoryId)
                    {
                        products.Categories.Add(db.Categories.Find(item));
                    }
                }

                if (imgFiles != null)
                {
                    if (imgFiles[0] != null)
                    {
                        foreach (var item in imgFiles)
                        {
                            products.ProductImages.Add(

                         new ProductImages { ImageUrl = ImageUpload.SaveImage(item, 746, 551), IsActive = products.IsActive, ProductId = products.ProductId, RegisterDate = DateTime.Now });
                        }
                    }
                }

                products.RegisterDate = DateTime.Now;
                products.ProductNearPlace.RegisterDate = DateTime.Now;
                products.ProductNearPlace.IsActive = true;
                products.ProductAddresses.ProvinceId = province.ProvinceId;
                products.ProductAddresses.DistrictId = district.DistrictId;
                products.ProductAddresses.TownId = towns.TowndId;
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.ProductAddresses, "ProductId", "Adress", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductDetails, "ProductId", "RoomCount", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductNearPlace, "ProductId", "Education", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductProperties, "ProductId", "ProductId", products.ProductId);
            return View(products);
        }
   
        public ActionResult imageDelete(int imageId, int id)/// tekli resim silmek isterse !!! x iconuna tıklayacak, Url olarak bu action'a gelecek ve resmi silecek daha sonra verdiğim product ıdye geri  gidecek! Test yapamadım resim yuklemediğimiz için
        {
            var resim = db.ProductImages.Find(imageId);
            ImageUpload.DeleteByPath(resim.ImageUrl);
            db.ProductImages.Remove(resim);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ManagementPanel/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.ProductAddresses, "ProductId", "Adress", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductDetails, "ProductId", "RoomCount", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductNearPlace, "ProductId", "Education", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductProperties, "ProductId", "ProductId", products.ProductId);
            ViewBag.CategoryId = db.Categories.ToList();
            //ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Name", products.ProductAddresses.ProvinceId);
            //ViewBag.DistrictId = new SelectList(db.District, "DistrictId", "Name", products.ProductAddresses.DistrictId);
            //ViewBag.TowndId = new SelectList(db.Towns, "TowndId", "Name", products.ProductAddresses.TownId);
            tdp.Sehirler = new SelectList(db.Towns, "TowndId", "Name",products.ProductAddresses.TownId);
            tdp.İlceler = new SelectList(db.District, "DistrictId", "Name",products.ProductAddresses.DistrictId);
            tdp.Bölgeler = new SelectList(db.Province, "ProvinceId", "Name", products.ProductAddresses.ProvinceId);
            ViewBag.tdp = tdp;
            return View(products);
        }

        // POST: ManagementPanel/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ProductId,Title,SubTitle,Description,Price,Metrekare,IsActive,RegisterDate,Deposit,ProductAddresses,ProductDetails,ProductNearPlace,ProductProperties")] Products products, Province province, District district, Towns towns, List<int> CategoryId, List<HttpPostedFileBase> imgFiles)
        {
            if (ModelState.IsValid)
            {
                Products update = db.Products.Find(products.ProductId);

                if (CategoryId.Count() > 0)
                {
                    update.Categories.Clear();
                    foreach (var item in CategoryId)
                    {
                        update.Categories.Add(db.Categories.Find(item));
                    }
                }

                //if (imgFiles != null)
                //{
                //    foreach (var item in imgFiles)
                //    {
                //        update.ProductImages.Add(

                //     new ProductImages { ImageUrl = ImageUpload.SaveImage(item, 746, 551), IsActive = products.IsActive, ProductId = update.ProductId, RegisterDate = DateTime.Now });
                //    }
                //}
                if (imgFiles != null)
                {
                 
                    if (imgFiles[0] != null)
                    {
                        foreach (var item in imgFiles)
                        {
                            update.ProductImages.Add(

                         new ProductImages { ImageUrl = ImageUpload.SaveImage(item, 746, 551), IsActive = products.IsActive, ProductId = update.ProductId, RegisterDate = DateTime.Now });
                        }
                    }
                }



                update.Title = products.Title;
                update.SubTitle = products.SubTitle;
                update.Description = products.Description;
                update.Price = products.Price;
                update.Metrekare = products.Metrekare;
                update.IsActive = products.IsActive;
                update.RegisterDate = DateTime.Now;
                update.Deposit = products.Deposit;
                update.ProductAddresses.Adress = products.ProductAddresses.Adress;
                update.ProductAddresses.ProvinceId = province.ProvinceId;
                update.ProductAddresses.DistrictId = district.DistrictId;
                update.ProductAddresses.TownId = towns.TowndId;
                update.ProductDetails.RoomCount = products.ProductDetails.RoomCount;
                update.ProductDetails.BathCount = products.ProductDetails.BathCount;
                update.ProductDetails.BalconyCount = products.ProductDetails.BalconyCount;
                update.ProductDetails.GarageCount = products.ProductDetails.GarageCount;
                update.ProductDetails.PoolCount = products.ProductDetails.PoolCount;
                update.ProductDetails.PersonCount = products.ProductDetails.PersonCount;
                update.ProductNearPlace.Education = products.ProductNearPlace.Education;
                update.ProductNearPlace.EducationDistance = products.ProductNearPlace.EducationDistance;
                update.ProductNearPlace.Hospital = products.ProductNearPlace.Hospital;
                update.ProductNearPlace.HospitalDistance = products.ProductNearPlace.HospitalDistance;
                update.ProductNearPlace.TransportationType = products.ProductNearPlace.TransportationType;
                update.ProductNearPlace.TransportationDisance = products.ProductNearPlace.TransportationDisance;
                update.ProductNearPlace.IsActive = true;
                update.ProductNearPlace.RegisterDate = DateTime.Now;
                update.ProductProperties.Barbecue = products.ProductProperties.Barbecue;
                update.ProductProperties.Tv = products.ProductProperties.Tv;
                update.ProductProperties.Klima = products.ProductProperties.Klima;
                update.ProductProperties.Wifi = products.ProductProperties.Wifi;
                update.ProductProperties.Sauna = products.ProductProperties.Sauna;
                update.ProductProperties.Fridge = products.ProductProperties.Fridge;
                update.ProductProperties.Washer = products.ProductProperties.Washer;
                update.ProductProperties.Gym = products.ProductProperties.Gym;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.ProductAddresses, "ProductId", "Adress", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductDetails, "ProductId", "RoomCount", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductNearPlace, "ProductId", "Education", products.ProductId);
            ViewBag.ProductId = new SelectList(db.ProductProperties, "ProductId", "ProductId", products.ProductId);
            return View(products);
        }

        // GET: ManagementPanel/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: ManagementPanel/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            ProductDetails details = db.ProductDetails.Find(id);
            ProductAddresses address = db.ProductAddresses.Find(id);
            ProductProperties property = db.ProductProperties.Find(id);
            ProductNearPlace place = db.ProductNearPlace.Find(id);
            foreach (var item in products.ProductImages.ToList())
            {
                ImageUpload.DeleteByPath(item.ImageUrl);
                db.ProductImages.Remove(item);
            }
            foreach (var item in products.FavoriProducts.ToList())
            {
                db.FavoriProducts.Remove(item);
            }
            foreach (var item in products.Sales.ToList())
            {
                db.Sales.Remove(item);
            }

            db.ProductDetails.Remove(details);
            db.ProductAddresses.Remove(address);
            db.ProductProperties.Remove(property);
            db.ProductNearPlace.Remove(place);
            products.Categories.Clear();
            db.Products.Remove(products);
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
