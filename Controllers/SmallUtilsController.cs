using log4net;
using WebUtils.Web.Helpers;
using WebUtils.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUtils.Web.Controllers
{
    public class SmallUtilsController : Controller
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

        // GET: SmallUtils
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BllDal()
        {
            return View();
        }

        // GET: /SmallUtils/RemoveDups
        public ActionResult RemoveDups()
        {
            return View();
        }

        // GET: /SmallUtils/RemoveDups
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult RemoveDups(PostedTextareaModel model)
        {
            string[] lines = model.Text.Split(new char[] { '\n' });
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();

            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    //Logger.DebugFormat("Line #{0}: {1}", i, lines[i].Replace("\t", "{TAB}"));
                    if (String.IsNullOrEmpty(lines[i]) == false)
                    {
                        string[] ar = lines[i].Split(new char[] { '\t' });
                        var existing = list.FirstOrDefault(x => x.Item1.Equals(ar[0].Trim()));
                        if (existing == null)
                        {
                            list.Add(new Tuple<string, string>(ar[0].Trim(), ar[1].Trim()));
                        }
                        else if (existing.Item2.Equals(ar[1].Trim()) == false)
                        {
                            list.Add(new Tuple<string, string>(ar[0].Trim(), ar[1].Trim()));
                        }
                        else
                        {
                            Logger.WarnFormat("Line #{0} is duplicated: {1}", i, lines[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error removing duplicates", ex);
                throw;
            }

            Logger.InfoFormat("There are {0} lines without duplicates, out of {1}", list.Count, lines.Length);

            StringBuilder sb = new StringBuilder();
            foreach (var item in list.OrderBy(x => x.Item1))
            {
                sb.AppendFormat("{0}\t{1}", item.Item1, item.Item2);
                sb.AppendLine();
            }
            

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "duplicatesremoved.csv");
        }

        // GET: /SmallUtils/CountUniqueLogs
        public ActionResult CountUniqueLogs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CountUniqueLogs(PostedFileModel model)
        {
            try
            {
                var p = new ApacheLogsParser();
                string s;

                int total = 0;
                List<WebVisitorModel> visitors = new List<WebVisitorModel>();
                WebVisitorModel v = null;

                using (StreamReader sr = new StreamReader(Request.Files[0].InputStream))
                {
                    while ((s = sr.ReadLine()) != null)
                    {
                        total++;
                        try
                        {
                            v = p.ParseLine(s);
                            visitors.Add(v);
                        }
                        catch (Exception pex)
                        {
                            Logger.Error("Error parsing line: " + s, pex);
                        }
                    }
                }

                Logger.InfoFormat("Parsed {0} visitors out of {1} lines", visitors.Count, total);

                var ipAndDate = visitors.Select(x => String.Concat(x.Ip, "_", x.VisitDate.ToString("yyyyMMdd_HHmm"))).ToArray();
                //for (int i = 0; i < 20; i++)
                //{
                //    Logger.Debug(ipAndDate[i]);
                //}
                Logger.InfoFormat("More less {0} unique visits", ipAndDate.Distinct().Count());

                return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Error parsing logs file", ex);
                throw;
            }
        }

    }
}