using WebUtils.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebUtils.Web.Helpers
{
    public class ApacheLogsParser
    {
        public WebVisitorModel ParseLine(string s)
        {
            // Expected: 78.57.216.1 - - [25/Oct/2015:01:02:29 -0500] "GET /js/jquery-1.5.1.min.js HTTP/1.1" 200 85275 "http://www.host.com/some/script?params" "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36"
            WebVisitorModel v = new WebVisitorModel();

            int start = 0;
            int end = 0;

            end = s.IndexOf(' ');
            v.Ip = GetIp(s.Substring(0, end));

            start = s.IndexOf('[');
            end = s.IndexOf(']');
            v.VisitDate = GetVisitDate(s.Substring(start, end - start));

            start = s.IndexOf('"', end);
            end = s.IndexOf('"', start + 1);
            v.ResourceVisited = GetVisitedResource(s.Substring(start, end - start));

            v.HttpResponseCode = int.Parse(s.Substring(end + 2, 3));

            v.BytesDownloaded = ParseBytesDownloaded(s.Substring(end + 6));

            // In case we have referer and user-agent
            start = s.IndexOf('"', end + 1);
            if (start > 0)
            {
                // Referer
                end = s.IndexOf('"', start + 1);
                v.RefererUrl = s.Substring(start, end - start).Trim(new char[] { '"' });

                // User-agent after referer
                start = s.IndexOf('"', end + 1);
                if (start > 0)
                {
                    end = s.LastIndexOf('"');
                    v.UserAgent = s.Substring(start + 1, end - start - 1);
                }
            }

            return v;
        }

        private int ParseBytesDownloaded(string s)
        {
            int end = s.IndexOf(' ');
            if (Char.IsDigit(s[0]) == false)
            {
                return 0;
            }
            return int.Parse(s.Substring(0, end));
        }

        private string GetVisitedResource(string s)
        {
            // Expected: "GET /index.php/lt/43/audiniai HTTP/1.1"

            string r = null;

            if (s.StartsWith("\""))
            {
                s = s.Substring(1);
            }

            int start = 0;
            if (s.StartsWith("GET ", StringComparison.OrdinalIgnoreCase))
            {
                start = 4;
            }
            else if (s.StartsWith("POST ", StringComparison.OrdinalIgnoreCase))
            {
                start = 5;
            }

            // Skip HTTP/1.x
            int end = s.LastIndexOf(' ');

            r = s.Substring(start, end - start);

            return r;
        }

        private DateTime GetVisitDate(string s)
        {
            // Expected such: [25/Oct/2015:00:00:53 -0500]

            DateTime dt = DateTime.MinValue;

            if (s.StartsWith("["))
            {
                s = s.Substring(1);
            }
            if (s.EndsWith("]"))
            {
                s = s.Substring(0, s.Length - 1);
            }

            int pos = s.IndexOf(':');
            if (pos > 0)
            {
                s = String.Concat(s.Substring(0, pos), " ", s.Substring(pos + 1));
                dt = DateTime.Parse(s, CultureInfo.InvariantCulture);
            }
            return dt;
        }

        private string GetIp(string s)
        {
            if (Char.IsDigit(s[0]))
            {
                return s.Trim();
            }
            return null;
        }
    }
}
