//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ToutorialBlog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAccessLevel
    {
        public int AccessLevelId { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<bool> CreateAccess { get; set; }
        public Nullable<bool> EditAccess { get; set; }
        public Nullable<bool> DeleteAccess { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> ApproveAccess { get; set; }
    
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        public virtual tblUser tblUser2 { get; set; }
        public virtual tblMenu tblMenu { get; set; }
        public virtual tblRole tblRole { get; set; }
    }
}
