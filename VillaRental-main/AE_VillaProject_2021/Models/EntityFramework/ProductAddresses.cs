//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AE_VillaProject_2021.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductAddresses
    {
        public int ProductId { get; set; }
        public string Adress { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int TownId { get; set; }
    
        public virtual District District { get; set; }
        public virtual Products Products { get; set; }
        public virtual Province Province { get; set; }
        public virtual Towns Towns { get; set; }
    }
}
