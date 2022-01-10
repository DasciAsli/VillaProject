using AE_VillaProject_2021.Models;
using AE_VillaProject_2021.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE_VillaProject_2021.Areas.ManagementPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CalendarController : Controller
    {
        VillaContext db = new VillaContext();
        // GET: ManagementPanel/Calendar
        public ActionResult Index()
        {
            return View();
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

                }) ;
            }
            return ec;
        }

    }
}