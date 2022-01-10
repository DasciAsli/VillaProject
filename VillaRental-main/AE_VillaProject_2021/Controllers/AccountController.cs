using AE_VillaProject_2021.Models.EntityFramework;
using PA_BlogProject_2021.Areas.ManagementPanel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AE_VillaProject_2021.Controllers
{
    [Authorize(Roles = "User")]
    public class AccountController : Controller
    {

        VillaContext db = new VillaContext();
        // GET: Account
        [HttpGet]
        public ActionResult AccountProfile()
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AccountProfile([Bind(Include = "Name,SurName,Email,Password,IsActive,RegisterDate,UserDetails")] Users users, HttpPostedFileBase imgFile)
        {
            if (ModelState.IsValid)
            {
                Users update = db.Users.FirstOrDefault(X => X.Name == users.Name);
                if (imgFile != null)
                {
                    ImageUpload.DeleteByPath(update.UserDetails.ImageUrl);
                    update.UserDetails.ImageUrl = ImageUpload.SaveImage(imgFile, 80, 83);
                }
                update.Name = users.Name;
                update.SurName = users.SurName;
                update.Email = users.Email;
                var hash = users.Password.GetHashCode().ToString();
                update.Password = hash;
                update.IsActive = true;
                update.RegisterDate = DateTime.Now;
                foreach (var item in db.Roles)
                {
                    if (item.RoleName == "User")
                    {
                        update.RoleId = item.RoleId;
                    }
                }
                update.UserDetails.Age = users.UserDetails.Age;
                update.UserDetails.PhoneNumber = users.UserDetails.PhoneNumber;
                update.UserDetails.Address = users.UserDetails.Address;
                update.UserDetails.Tc = users.UserDetails.Tc;
                update.UserDetails.PassportNumber = users.UserDetails.PassportNumber;
                db.SaveChanges();
                return RedirectToAction("AccountProfile", "Account");
            }
            return View(users);
        }
        public ActionResult AccountDelete(int id)
        {
            Users user = db.Users.FirstOrDefault(x => x.UserId == id);
            UserDetails detail = db.UserDetails.FirstOrDefault(x => x.UserId == id);
            foreach (var item in user.FavoriProducts.ToList())
            {
                db.FavoriProducts.Remove(item);
            }
            foreach (var item in user.Sales.ToList())
            {
                db.Sales.Remove(item);
            }
            if (detail.ImageUrl != null)
            {
                ImageUpload.DeleteByPath(detail.ImageUrl);
            }
            db.UserDetails.Remove(detail);
            db.Users.Remove(user);
            db.SaveChanges();
            FormsAuthentication.SignOut();
            Session.Remove("Name");
            return RedirectToAction("Index", "Home");
        }
        public ActionResult AccountFavourite(int ProductId)
        {
            var username = (string)Session["Name"];
            if (ProductId != 0 && username != null)
            {
                Products product = db.Products.FirstOrDefault(x => x.ProductId == ProductId);
                Users user = db.Users.FirstOrDefault(x => x.Name == username);
                if (user.FavoriProducts.Any(x=>x.ProductId==product.ProductId))
                {
                    db.FavoriProducts.Remove(user.FavoriProducts.FirstOrDefault(x => x.ProductId == product.ProductId));                   
                }
                else
                {
                    FavoriProducts fp = new FavoriProducts();
                    fp.Products = product;
                    fp.Users = user;
                    fp.IsActive = true;
                    fp.RegisterDate = DateTime.Now;
                    user.FavoriProducts.Add(fp);
                    db.FavoriProducts.Add(fp);
                }

            }

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult AccountSavedHomes()
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).FirstOrDefault();
            var favoriproducts = db.FavoriProducts.Where(x => x.UserId == user.UserId).ToList();
            ViewBag.User = user;
            return View(favoriproducts);
        }
        public ActionResult AccountSavedRental()
        {
            var username = (string)Session["Name"];
            var user = db.Users.Where(x => x.Name == username).FirstOrDefault();
            var rentalproducts = db.Sales.Where(x => x.UserId == user.UserId).OrderByDescending(x=>x.EntryDate).ToList();
            ViewBag.User = user;
            return View(rentalproducts);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Create(string Name, string Surname, string Email, string Password)
        {
            Users user = new Users();
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Surname) && !String.IsNullOrEmpty(Email) && !String.IsNullOrEmpty(Password))
            {
                user.Name = Name;
                user.SurName = Surname;
                user.Email = Email;
                var hash = Password.GetHashCode().ToString();
                user.Password = hash;
                user.IsActive = true;
                user.RegisterDate = DateTime.Now;
                foreach (var item in db.Roles)
                {
                    if (item.RoleName == "User")
                    {
                        user.RoleId = item.RoleId;
                    }
                }
                UserDetails detail = new UserDetails();
                detail.Gender = null;
                detail.Age = null;
                detail.PhoneNumber = null;
                detail.ImageUrl = null;
                detail.Address = null;
                detail.Tc = null;
                detail.PassportNumber = null;
                user.UserDetails = detail;
                db.UserDetails.Add(detail);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Contact", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string Name, string Password)
        {
            if (Name != null && Password != null)
            {
                var hash = Password.GetHashCode().ToString();
                Users user = db.Users.FirstOrDefault(u => u.Password == hash && u.Name == Name);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Name, false);
                    Session["Name"] = user.Name;

                    if (user.Roles.RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "Abouts", new { area = "ManagementPanel" });
                    }
                    else
                    {
                        return RedirectToAction("AccountProfile", "Account", new { area = "" });
                    }

                }

            }
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Remove("Name");
            return RedirectToAction("Index", "Home");
        }

    }

}
