using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebUtils.Web.Helpers
{
    public class ResourcesHelper
    {
        public CultureInfo ParseResourceFilename(string n)
        {
            if (String.IsNullOrEmpty(n))
            {
                throw new ArgumentNullException("Resource file name is empty");
            }

            if (n.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) == false)
            {
                throw new ArgumentException("Resource file name should ends with `.resx`");
            }

            // Cut ".resx"
            string s = n.Substring(0, n.Length - 5);
            int pos = s.LastIndexOf('.');

            if (pos == -1)
            {
                return new CultureInfo("en");
            }

            return new CultureInfo(s.Substring(pos + 1));
        }

        public string StripTabs(string s)
        {
            return s.Replace('\t', ' ');
        }
    }
}
