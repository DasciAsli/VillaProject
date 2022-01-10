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
    public class MessagesController : Controller
    {
        private VillaContext db = new VillaContext();

        // GET: ManagementPanel/Messages
        public ActionResult Index()
        {
            return View(db.Messages.ToList());
        }

        public ActionResult Sidebar()
        {
            var value = db.Messages.Where(x => x.IsActive == false).ToList();
            return View(value);
        }

        // GET: ManagementPanel/Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages messages = db.Messages.Find(id);
            messages.IsActive = true;
            db.SaveChanges();
            if (messages == null)
            {
                return HttpNotFound();
            }
            return View(messages);
        }

        // GET: ManagementPanel/Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Messages messages = db.Messages.Find(id);
            if (messages == null)
            {
                return HttpNotFound();
            }
            return View(messages);
        }

        // POST: ManagementPanel/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Messages messages = db.Messages.Find(id);
            db.Messages.Remove(messages);
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
