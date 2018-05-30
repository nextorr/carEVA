using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace carEVA.Areas.StudyArea.Controllers
{
    public class miniGamesController : Controller
    {
        // GET: StudyArea/miniGames
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult infograph() {
            return View();
        }
    }
}