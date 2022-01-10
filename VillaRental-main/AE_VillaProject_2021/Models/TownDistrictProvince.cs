using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AE_VillaProject_2021.Models
{
    public class TownDistrictProvince
    {
        public IEnumerable<SelectListItem> Sehirler { get; set; }
        public IEnumerable<SelectListItem> İlceler { get; set; }
        public IEnumerable<SelectListItem> Bölgeler { get; set; }
    }
}