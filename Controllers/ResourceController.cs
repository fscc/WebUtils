using log4net;
using Models;
using WebUtils.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUtils.Web.Controllers
{

    public class ResourceController : Controller
    {
        private ILog _logger = null;
        private ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(GetType());
                    log4net.Config.XmlConfigurator.Configure();
                }
                return _logger;
            }
        }

        // GET: Resources
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Resource/Export
        public ActionResult Export()
        {
            IList<LanguageResourcesList> languages = Session["__Languages"] as IList<LanguageResourcesList>;
            if (languages == null)
            {
                return RedirectToAction("Index");
            }

            ResourcesHelper helper = new ResourcesHelper();
            StringBuilder sb = new StringBuilder();
            string filename = String.Concat(String.Join("_", languages.Select(x => x.CultureName)), ".csv");

            sb.Append("KEY\t");
            for (int i = 0; i < languages.Count; i++)
            {
                sb.AppendFormat("{0}\t", languages[i].CultureName.ToUpper());
            }
            sb.AppendLine();

            LanguageResourcesList.Resource translated = null;
            foreach (var r in languages[0].Resources)
            {
                sb.AppendFormat("{0}\t{1}\t", r.Key, helper.StripTabs(r.Value));
                for (int i = 1; i < languages.Count; i++)
                {
                    translated = languages[i].Resources.FirstOrDefault(x => x.Key == r.Key);
                    sb.AppendFormat("{0}\t", translated == null ? "" : helper.StripTabs(translated.Value));
                }
                sb.AppendLine();
            }

            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", $"attachment; filename={filename}");
            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv");
        }

        // POST: /Resource/Parse
        [HttpPost]
        public ActionResult Parse()
        {
            var resxFiles = new List<HttpPostedFileBase>();
            Logger.InfoFormat("Uploaded {0} resource files to parse", Request.Files.Count);
            for (int i = 0; i < Request.Files.Count; i++)
            {
                if (Request.Files[i] != null && Request.Files[i].FileName.EndsWith(".resx"))
                {
                    resxFiles.Add(Request.Files[i]);
                }
            }
            var languages = ParseResx(resxFiles);

            Session["__Languages"] = languages.ToArray();

            return View(languages.ToArray());
        }

        private IList<LanguageResourcesList> ParseResx(IList<HttpPostedFileBase> files)
        {
            Logger.InfoFormat("  there are {0} language resources", files.Count);
            List<LanguageResourcesList> languages = new List<LanguageResourcesList>();
            var helper = new ResourcesHelper();
            foreach (var f in files)
            {
                try
                {
                    var ci = helper.ParseResourceFilename(f.FileName);
                    Logger.InfoFormat("  Culture of resources file `{0}` is: {1}", f.FileName, ci);
                    using (var r = new ResXResourceReader(f.InputStream))
                    {
                        var rl = new LanguageResourcesList(r);
                        rl.CultureName = ci.TwoLetterISOLanguageName;
                        Logger.InfoFormat("  Resources file {0} has {1} records", f.FileName, rl.Resources.Count);
                        languages.Add(rl);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error parsing `{f.FileName}`", ex);
                }
            }
            Logger.InfoFormat("  Got languages to prepare table: {0}", String.Join(", ", languages.Select(x => x.CultureName)));

            return languages;
        }

    }
}
