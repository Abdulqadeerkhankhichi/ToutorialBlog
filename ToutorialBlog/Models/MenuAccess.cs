using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToutorialBlog.Models
{
    public class MenuAccess
    {
        public tblMenu Menu { get; set; }
        public bool? EditAccess { get; set; }
        public bool? DeleteAccess { get; set; }
        public bool? CreateAccess { get; set; }
        public bool? isActive { get; set; }
        public bool? ApproveAccess { get; set; }
        public int? RoleId { get; set; }
        public int MenuId { get; set; }


        
    }
}