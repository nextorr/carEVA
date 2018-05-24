using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace carEVA.Areas.StudyArea.Controllers
{
    public class dashboardController : Controller
    {
        // GET: StudyArea/dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}