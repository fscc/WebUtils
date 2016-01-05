using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUtils.Web.Models
{
    public class PostedTextareaModel
    {
        [AllowHtml]
        public string Text { get; set; }
    }
}