using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToutorialBlog.Models
{
    public class HistoryArticle
    {
        public tblArticle Article { get; set; }
        public tblHistory History { get; set; }
    }
}