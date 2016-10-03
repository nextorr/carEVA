using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using carEVA.Utils;
using System.Net;

namespace carEVA.Controllers
{
    public class AdminController : Controller
    {
        private carEVAContext db = new carEVAContext();
        // GET: utils
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult syncScoreCounters()
        {

            if (userUtils.syncAllCounters(db) <= 0)
            {
                return RedirectToAction("Index");
            }
                
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error al modificar los contadores de usuario");
        }
    }
}