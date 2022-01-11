using AE_VillaProject_2021.Models;
using AE_VillaProject_2021.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using PagedList;
using PagedList.Mvc;

namespace AE_VillaProject_2021.Controllers
{
    [AllowAnonymous] //Globalde yaptığımız kısıtlamanın bu sayfada geçerli olmaması için 
    public class HomeController : Controller
    {
        VillaContext db = new VillaContext();
        TownDistrictProvince tdp = new TownDistrictProvince();

        public ActionResult Index()
        {
            tdp.Sehirler = new SelectList(db.Towns.Where(y => y.IsActive == true).ToList(), "TowndId", "Name");
            tdp.İlceler = new SelectList(db.District.Where(z => z.IsActive == true).ToList(), "DistrictId", "Name");
            var categories = db.Categories.Where(x => x.IsActive == true).ToList();
            var products = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.ProductId).Take(6).ToList();
            ViewBag.tdp = tdp;
            ViewBag.Category = new SelectList(db.Categories.Where(x=>x.IsActive== true).ToList(), "CategoryId", "Name");
            var user = (string)Session["Name"];
            ViewBag.User = db.Users.Where(x => x.Name == user).ToList();
            return View(products);
        }


        public JsonResult ilcegetir(int p)
        {
            var ilceler = (from x in db.District
                           join y in db.Towns on x.Towns.TowndId equals y.TowndId
                           where x.Towns.TowndId == p && x.IsActive==true
                           select new
                           {
                               Text = x.Name,
                               Value = x.DistrictId.ToString()
                           }).ToList();
            return Json(ilceler, JsonRequestBehavior.AllowGet);
        }
        public JsonResult bölgegetir(int p)
        {
            var bölgeler = (from x in db.Province
                            join y in db.District on x.District.DistrictId equals y.DistrictId
                            where x.District.DistrictId == p && x.IsActive == true
                            select new
                            {
                                Text = x.Name,
                                Value = x.ProvinceId.ToString()
                            }).ToList();
            return Json(bölgeler, JsonRequestBehavior.AllowGet);
        }


        public ActionResult villagetir(int town, int district, int category)
        {
            var model = db.Products.Where(p => p.IsActive == true).ToList();
            if (town != 0 && district != 0 && model.Any(b => b.Categories.Any(t => t.CategoryId == category)))
            {
                model = db.Products.Where(p => p.Categories.Any(t => t.CategoryId == category) && p.ProductAddresses.TownId == town && p.ProductAddresses.DistrictId == district).ToList();
            }
            else
            {
                model = db.Products.Where(p => p.IsActive == true).ToList();
            }
            var user = (string)Session["Name"];
            ViewBag.User = db.Users.Where(x => x.Name == user).ToList();
            return PartialView("_VillaGetir", model);
        }


        public ActionResult tümvillalar()
        {
            var user = (string)Session["Name"];
            ViewBag.User = db.Users.Where(x => x.Name == user).ToList();
            var model = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.ProductId).ToList();
            return PartialView("_VillaGetir", model);
        }

        public ActionResult VillaList(int? page, int? id)
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).ToList();
            ViewBag.User = user;
            tdp.Sehirler = new SelectList(db.Towns.Where(x=>x.IsActive==true).ToList(), "TowndId", "Name");
            tdp.İlceler = new SelectList(db.District.Where(x => x.IsActive == true).ToList(), "DistrictId", "Name");
            tdp.Bölgeler = new SelectList(db.Province.Where(x => x.IsActive == true).ToList(), "ProvinceId", "Name");
            ViewBag.tdp = tdp;
            var model = db.Products.Where(u => u.IsActive == true).ToList();
            if (page == null || page.Value == 1)
            {
                if (id == null)
                {
                    ViewBag.Pagelist = model.ToList().ToPagedList(1, 6);
                    return View(model.ToPagedList(1, 6));
                }
                else
                {
                    var yok = db.Products.Where(u => u.IsActive == true).Where(u => u.Categories.Any(p => p.CategoryId == id.Value)).ToList();
                    ViewBag.Pagelist = yok.ToList().ToPagedList(1, 6);
                    return View(yok.ToPagedList(1, 6));
                }
            }
            else
            {
                if (id == null)
                {
                    ViewBag.Pagelist = model.ToList().ToPagedList(1, 6);
                    return View(model.ToPagedList(1, 6));
                }
                else
                {
                    var gelen = db.Categories.Where(u => u.IsActive == true).Where(u => u.CategoryId == id.Value).ToList();
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(gelen.ToPagedList(page.Value, 6));
                }

            }

        }
        [HttpPost]
        public ActionResult VillaList(VillaList villaList, int? page)
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).ToList();
            ViewBag.User = user;
            if (page == null)
            {
                page = 1;
            }
            tdp.Sehirler = new SelectList(db.Towns, "TowndId", "Name");
            tdp.İlceler = new SelectList(db.District, "DistrictId", "Name");
            tdp.Bölgeler = new SelectList(db.Province, "ProvinceId", "Name");
            ViewBag.tdp = tdp;
            //kat sayımız yok !!!!
            var model = db.Products.Where(u => u.IsActive == true).ToList();
            if (villaList.kisiSayisi != null && villaList.yatakSayisi != null && villaList.banyo != null && villaList.Kat != null && villaList.metrekare != null && villaList.TowndId != 0 && villaList.ProvinceId != 0 && villaList.DistrictId != 0 && villaList.min != null && villaList.max != null)
            {
                var gelen = model.Where(u =>
            u.ProductDetails.PersonCount == (villaList.kisiSayisi.Value).ToString() &&
            u.ProductDetails.BathCount == (villaList.banyo.Value).ToString() &&
            u.ProductDetails.RoomCount == villaList.yatakSayisi.Value.ToString() && u.Metrekare == villaList.metrekare.Value.ToString() && u.ProductAddresses.DistrictId == villaList.DistrictId && u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.ProvinceId == villaList.ProvinceId);
                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");

                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }
            }
            else if (villaList.kisiSayisi != null && villaList.yatakSayisi != null && villaList.TowndId != 0 && villaList.ProvinceId != 0 && villaList.DistrictId != 0)
            {
                var gelen = model.Where(u =>
                    u.ProductDetails.PersonCount == (villaList.kisiSayisi.Value).ToString() &&
                    u.ProductDetails.RoomCount == villaList.yatakSayisi.Value.ToString() && u.ProductAddresses.DistrictId == villaList.DistrictId && u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.ProvinceId == villaList.ProvinceId);
                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");

                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }

            }
            else if (villaList.kisiSayisi != null && villaList.TowndId != 0 && villaList.ProvinceId != 0 && villaList.DistrictId != 0)
            {

                var gelen = model.Where(u =>
                      u.ProductDetails.PersonCount == (villaList.kisiSayisi.Value).ToString() && u.ProductAddresses.DistrictId == villaList.DistrictId && u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.ProvinceId == villaList.ProvinceId);
                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);

                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));

                }

            }
            else if (villaList.TowndId != 0 && villaList.ProvinceId != 0 && villaList.DistrictId != 0)
            {
                var gelen = model.Where(u => u.ProductAddresses.DistrictId == villaList.DistrictId && u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.ProvinceId == villaList.ProvinceId);

                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6); ;
                    return View(gelen.ToList().ToPagedList(page.Value, 6)); ;
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }


            }
            else if (villaList.TowndId != 0 && villaList.ProvinceId != 0)
            {
                var gelen = model.Where(u => u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.ProvinceId == villaList.ProvinceId);
                if (gelen.Count() > 0)

                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);

                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }
            }
            else if (villaList.TowndId != 0 && villaList.DistrictId != 0)
            {
                var gelen = model.Where(u => u.ProductAddresses.TownId == villaList.TowndId && u.ProductAddresses.DistrictId == villaList.DistrictId);
                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);

                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }
            }
            else if (villaList.TowndId != 0)
            {
                var gelen = model.Where(u => u.ProductAddresses.TownId == villaList.TowndId);
                if (gelen.Count() > 0)
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    return View(gelen.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    ViewBag.Pagelist = gelen.ToList().ToPagedList(1, 6);
                    ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                    return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
                }
            }
            else
            {
                ViewBag.Pagelist = db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6);
                ViewBag.Message = string.Format("Aradığınız Villa Bulunamadı..");
                return View(db.Products.Where(u => u.IsActive == true).ToList().ToPagedList(page.Value, 6));
            }
        }
        public ActionResult Sirala(int? page, int? sirala)
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).ToList();
            ViewBag.User = user;
            tdp.Sehirler = new SelectList(db.Towns, "TowndId", "Name");
            tdp.İlceler = new SelectList(db.District, "DistrictId", "Name");
            tdp.Bölgeler = new SelectList(db.Province, "ProvinceId", "Name");
            ViewBag.tdp = tdp;
            if (page == null)
            {
                page = 1;
            }
            if (page == null || page == 1)
            {
                if (sirala.Value == 0)
                {
                    var model = db.Products.Where(u => u.IsActive == true).OrderBy(u => u.Price).ToList();
                    ViewBag.PageList = model.ToList().ToPagedList(1, 6);
                    return View("VillaList", model.ToList().ToPagedList(1, 6));
                }
                else if (sirala.Value == 1)
                {
                    var model = db.Products.Where(u => u.IsActive == true).OrderByDescending(u => u.Price).ToList();
                    ViewBag.PageList = model.ToList().ToPagedList(page.Value, 6);
                    return View("VillaList", model.ToList().ToPagedList(page.Value, 6));
                }
                else
                {
                    var model = db.Products.Where(u => u.IsActive == true).OrderByDescending(u => u.Price).ToList();
                    List<Products> yenidizi1 = new List<Products>();
                    foreach (var item in model)
                    {
                        yenidizi1.Add(item);
                    }
                    List<Products> yenidizi2 = new List<Products>();
                    int favoriderecesi = 0;
                    foreach (var item in yenidizi1.ToList())
                    {                     
                        if (db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() <= favoriderecesi)
                        {
                            if (yenidizi2.Where(x=>x.ProductId==item.ProductId).Count()<1)
                            {
                                yenidizi2.Add(item);
                                yenidizi1.Remove(item);
                            }                  
                            foreach (var item2 in yenidizi1.ToList())
                            {
                                if (db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count()== db.FavoriProducts.Where(x => x.ProductId == item2.ProductId).ToList().Count())
                                {
                                    yenidizi2.Add(item2);
                                    yenidizi1.Remove(item2);
                                }
                            }
                            favoriderecesi = db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() + 1;
                        }
                        else
                        {
                            favoriderecesi = db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() + 1;
                        }
                        
                    }
                    
                    List<Products> yenidizi3 = new List<Products>();
                    foreach (var item in yenidizi2.ToList())
                    {
                        yenidizi3.Add(yenidizi2.Last());
                        yenidizi2.Remove(yenidizi2.Last());
                    }
                    model = yenidizi3;
                    ViewBag.PageList = model.ToList().ToPagedList(page.Value, 6);
                    return View("VillaList", model.ToList().ToPagedList(page.Value, 6));
                }


            }
            else
            {
                if (sirala.Value == 0)
                {
                    var model = db.Products.Where(u => u.IsActive == true).OrderBy(u => u.Price).ToList();
                    ViewBag.PageList = model.ToList().ToPagedList(1, 6);
                    return View("VillaList", model.ToList().ToPagedList(1, 6));
                }
                else if (sirala.Value == 1)
                {
                    var model = db.Products.Where(u => u.IsActive == true).OrderByDescending(u => u.Price).ToList();
                    ViewBag.PageList = model.ToList().ToPagedList(page.Value, 6);
                    return View("VillaList", model.ToList().ToPagedList(page.Value, 6));
                }
                else
                {

                    var model = db.Products.Where(u => u.IsActive == true).OrderByDescending(u => u.Price).ToList();
                    List<Products> yenidizi1 = new List<Products>();
                    foreach (var item in model)
                    {
                        yenidizi1.Add(item);
                    }
                    List<Products> yenidizi2 = new List<Products>();
                    int favoriderecesi = 0;
                    foreach (var item in yenidizi1.ToList())
                    {
                        if (db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() <= favoriderecesi)
                        {
                            yenidizi2.Add(item);
                            yenidizi1.Remove(item);
                            foreach (var item2 in yenidizi1.ToList())
                            {
                                if (db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() == db.FavoriProducts.Where(x => x.ProductId == item2.ProductId).ToList().Count())
                                {
                                    yenidizi2.Add(item2);
                                    yenidizi1.Remove(item2);
                                }
                            }

                            favoriderecesi = db.FavoriProducts.Where(x => x.ProductId == item.ProductId).ToList().Count() + 1;

                        }
                    }
                    List<Products> yenidizi3 = new List<Products>();
                    foreach (var item in yenidizi2.ToList())
                    {
                        yenidizi3.Add(yenidizi2.Last());
                        yenidizi2.Remove(yenidizi2.Last());
                    }
                    model = yenidizi3;

                    ViewBag.PageList = model.ToList().ToPagedList(page.Value, 6);
                    return View("VillaList", model.ToList().ToPagedList(page.Value, 6));
                }
            }


        }
        public ActionResult VillaDetails(int id)
        {
            var username = (string)Session["Name"];
            var durum = 0;
            if (id != 0 && username != null)
            {
                Products product = db.Products.FirstOrDefault(x => x.ProductId == id);
                Users user = db.Users.FirstOrDefault(x => x.Name == username);
                if (user.FavoriProducts.Any(x => x.ProductId == product.ProductId))
                {
                    durum = 1;
                }
                var toplamüyesayısı = db.Users.Count();
                var buvillakaçkişininfavorisi = db.FavoriProducts.Where(x => x.ProductId == product.ProductId).ToList();
                ViewBag.toplamüyesayısı = toplamüyesayısı;              
                ViewBag.favorioranı = buvillakaçkişininfavorisi.Count();
                ViewBag.Durum = durum;
                return View(product);
            }
            else if (id != 0 && username == null)
            {
                Products product = db.Products.FirstOrDefault(x => x.ProductId == id);
                var toplamüyesayısı = db.Users.Count();
                var buvillakaçkişininfavorisi = db.FavoriProducts.Where(x => x.ProductId == product.ProductId).ToList();
                ViewBag.toplamüyesayısı = toplamüyesayısı;
                ViewBag.favorioranı = buvillakaçkişininfavorisi.Count();
                ViewBag.Durum = durum;
                return View(product);
            }
           
            ViewBag.Sales = db.Sales.Where(x => x.IsActive == true).ToList();
            return RedirectToAction("Index", "Home");
        }
        public JsonResult GetCalendarEvents()
        {

            return Json(EventList(), JsonRequestBehavior.AllowGet);
        }
        public List<EventClass> EventList()
        {
            List<EventClass> ec = new List<EventClass>();
            List<string> colors = new List<string>()
            {"black","red","blue","green","yellow" };
            Random rnd = new Random();
            foreach (var item in db.Sales)
            {
                int index = rnd.Next(colors.Count());
                ec.Add(new EventClass()
                {
                    id = item.SalesId,
                    title = item.Products.Title,
                    start = item.EntryDate.ToString("s"),
                    end = item.ReleaseDate.ToString("s"),
                    allDay = true,
                    color = colors[index]

                });
            }
            return ec;
        }
        [HttpGet]
        public ActionResult Kirala(int id)
        {
            ViewBag.ProductId = new SelectList(db.Products.Where(x => x.ProductId == id), "ProductId", "Title");
            return View();
        }
        [HttpPost]
        public ActionResult Kirala(int id, DateTime entrydate, DateTime releasedate)
        {
            var username = (string)Session["Name"];
            var list = db.Sales.Where(x => x.IsActive == true && x.ProductId == id).ToList();
            var doluluksayacı = 0;
            double price = db.Products.FirstOrDefault(x => x.ProductId == id).Price;
            List<DateTime> aralıktakitarihler = new List<DateTime>();
            for (DateTime i = entrydate; i <= releasedate ; i=i.AddDays(1))
            {
                aralıktakitarihler.Add(i);
            }
            foreach (var item in list)
            {
                foreach (var item2 in aralıktakitarihler)
                {
                    if (item2 ==item.EntryDate || item2== item.ReleaseDate)
                    {
                        doluluksayacı++;
                    }
                }
           
            }
            if (username != null && doluluksayacı == 0)
            {
                Sales sales = new Sales();
                Users user = db.Users.FirstOrDefault(x => x.Name == username);
                sales.UserId = user.UserId;
                sales.ProductId = id;
                sales.EntryDate = entrydate;
                sales.ReleaseDate = releasedate;
                sales.RegisterDate = DateTime.Now;
                sales.IsActive = true;
                sales.TotalPrice = ((releasedate - entrydate).TotalDays) * price;
                db.Sales.Add(sales);
                db.SaveChanges();
                return PartialView("Bill", sales);
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(x => x.ProductId == id), "ProductId", "Title");
                var kiralıkvilla = db.Sales.FirstOrDefault(x => x.ProductId == id);
                ViewBag.Durum = "Belirttiğiniz tarih aralığı dolu.Lütfen yeni bir aralık belirleyiniz";
                return View(kiralıkvilla);
            }

            
            
            
        }
        public ActionResult CompareProperties()
        {
            return View();
        }

        public ActionResult About()
        {
            var aboutvalues = db.Abouts.Where(x => x.IsActive == true).OrderBy(x => x.AboutId).Take(1).ToList();
            return View(aboutvalues);
        }
        public ActionResult TermsAndConditions()
        {
            var termsvalues = db.Terms.Where(x => x.IsActive == true).ToList();
            return View(termsvalues);
        }
        public ActionResult Contact()
        {
            var user = (string)Session["Name"];
            if (user != null)
            {
                ViewBag.User = db.Users.Where(x => x.Name == user).ToList();
            }
            var contactvalues = db.Contacts.Where(x => x.IsActive == true).OrderBy(x => x.Contactıd).Take(1).ToList();
            return View(contactvalues);
        }
        public ActionResult Message(Messages p)
        {
            if (Session["Name"] == null)
            {
                p.IsActive = false;
                p.RegisterDate = DateTime.Now;
            }
            else
            {
                var user = (string)Session["Name"];
                Users userx = db.Users.Where(x => x.Name == user).FirstOrDefault();
                p.Name = userx.Name;
                p.Mail = userx.Email;
                p.Phone = userx.UserDetails.PhoneNumber;
                p.IsActive = false;
                p.RegisterDate = DateTime.Now;
            }

            db.Messages.Add(p);
            db.SaveChanges();
            return RedirectToAction("Contact");
        }
        public ActionResult Header()
        {
            var user = (string)Session["Name"];
            if (user != null)
            {
                string rolename = db.Users.Where(x => x.Name == user).FirstOrDefault().Roles.RoleName;
                ViewBag.Roles = rolename;
            }

            var contactvalues = db.Contacts.Where(x => x.IsActive == true).OrderBy(x => x.Contactıd).Take(1).ToList();
            return PartialView(contactvalues);
        }
        public ActionResult Footer()
        {
            var contactvalues = db.Contacts.Where(x => x.IsActive == true).OrderBy(x => x.Contactıd).Take(1).ToList();
            ViewBag.About = db.Abouts.Where(X => X.IsActive == true).OrderBy(x => x.AboutId).Take(1).ToList();
            if (db.Products.Count() >= 2)
            {
                ViewBag.Villa = db.Products.Where(x => x.IsActive == true).OrderByDescending(x => x.RegisterDate).Take(2).ToList();
            }
            else if (db.Products.Count() < 2)
            {
                ViewBag.Villa = db.Products.Where(x => x.IsActive == true).ToList();
            }

            return PartialView(contactvalues);
        }

    }
}