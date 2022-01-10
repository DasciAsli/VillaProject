using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE_VillaProject_2021.Models
{
    public class VillaList
    {
        public int TowndId { get; set; }
        public int DistrictId { get; set; }
        public int ProvinceId { get; set; }
        public int? kisiSayisi { get; set; }
        public int? yatakSayisi { get; set; }
        public int? banyo { get; set; }
        public int? Kat { get; set; }
        public int? metrekare { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
    }
}