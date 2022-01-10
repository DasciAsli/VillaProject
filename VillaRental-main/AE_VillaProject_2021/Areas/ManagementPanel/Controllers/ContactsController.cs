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
    public class ContactsController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Contacts
        public ActionResult Index()
        {
            return View(db.Contacts.ToList());
        }

        // GET: ManagementPanel/Contacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = db.Contacts.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // GET: ManagementPanel/Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementPanel/Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Contactıd,Adress,Email,PhoneNumber,InstagramUrl,TwitterUrl,YoutubeUrl,FacebookUrl,IsActive,RegisterDate")] Contacts contacts)
        {
            if (ModelState.IsValid)
            {
                contacts.RegisterDate = DateTime.Now;
                db.Contacts.Add(contacts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contacts);
        }

        // GET: ManagementPanel/Contacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = db.Contacts.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // POST: ManagementPanel/Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Contactıd,Adress,Email,PhoneNumber,InstagramUrl,TwitterUrl,YoutubeUrl,FacebookUrl,IsActive,RegisterDate")] Contacts contacts)
        {
            if (ModelState.IsValid)
            {
                Contacts update = db.Contacts.Find(contacts.Contactıd);              
                update.Adress = contacts.Adress;
                update.Email = contacts.Email;
                update.PhoneNumber = contacts.PhoneNumber;
                update.InstagramUrl = contacts.InstagramUrl;
                update.TwitterUrl = contacts.TwitterUrl;
                update.YoutubeUrl = contacts.YoutubeUrl;
                update.FacebookUrl = contacts.FacebookUrl;
                update.IsActive = contacts.IsActive;
                update.RegisterDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contacts);
        }

        // GET: ManagementPanel/Contacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacts contacts = db.Contacts.Find(id);
            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // POST: ManagementPanel/Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contacts contacts = db.Contacts.Find(id);
            db.Contacts.Remove(contacts);
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
