using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUtils.Web.Models
{
    public class TemporaryParsedResource
    {
        /// <summary>
        /// Like `ru-RU`
        /// </summary>
        public string CultureName { get; set; }

        public IList<LanguageResourcesList.Resource> Resources { get; set; }
    }
}
