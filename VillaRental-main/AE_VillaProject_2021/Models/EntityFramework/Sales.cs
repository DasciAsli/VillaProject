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
    
    public partial class Sales
    {
        public int SalesId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public System.DateTime EntryDate { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public bool IsActive { get; set; }
        public double TotalPrice { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Users Users { get; set; }
    }
}