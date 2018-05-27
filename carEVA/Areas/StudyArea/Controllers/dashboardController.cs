using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;

using carEVA.Utils;
using Microsoft.AspNet.Identity;

namespace carEVA.Areas.StudyArea.Controllers
{
    [Authorize]
    public class dashboardController : Controller
    {
        private carEVAContext db = new carEVAContext();
        // GET: StudyArea/dashboard
        
        public ActionResult Index()
        {
            //TODO: consider using a small view model
            string temp = User.Identity.GetUserId();
            ViewBag.publicKey = userUtils.publicKeyFromUserId(db, User.Identity.GetUserId());
            return View();
        }

        //GET: StudyArea/dashboard/LessonArea/courseID
        public ActionResult LessonArea(int id)
        {
            //TODO: consider using a small view model
            ViewBag.courseID = id;
            string temp = User.Identity.GetUserId();
            ViewBag.publicKey = userUtils.publicKeyFromUserId(db, User.Identity.GetUserId());
            return View();
        }
    }
}