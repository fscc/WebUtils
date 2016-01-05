using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUtils.Web.Models
{
    public class WebVisitorModel
    {
        public string Ip { get; set; }
        public DateTime VisitDate { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string ResourceVisited { get; set; }

        public int BytesDownloaded { get; set; }

        public int HttpResponseCode { get; set; }

        public string RefererUrl { get; set; }

        public string UserAgent { get; set; }
    }
}
