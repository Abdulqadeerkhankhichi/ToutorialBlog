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
    
    public partial class SearchArticle_Result
    {
        public int ArticleId { get; set; }
        public Nullable<int> Author { get; set; }
        public string Title { get; set; }
        public Nullable<int> Status { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> UnApprovedDate { get; set; }
        public Nullable<int> UnApprovedBy { get; set; }
        public string Tags { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<bool> isActive { get; set; }
    }
}
