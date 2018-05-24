using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using carEVA.Utils;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using carEVA.ViewModels;

namespace carEVA.Controllers
{
    //[Authorize(Roles = evaRoles.Admin)]
    public class AdminController : Controller
    {
        private carEVAContext db = new carEVAContext();
        private UserManager<ApplicationUser> userManager;
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

        public ActionResult createRoles()
        {
            RoleManager<IdentityRole> rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            //create the admin role if it does not exist
            if (!rm.RoleExists(evaRoles.admin))
            {
                rm.Create(new IdentityRole(evaRoles.admin));
            }
            if (!rm.RoleExists(evaRoles.user))
            {
                rm.Create(new IdentityRole(evaRoles.user));
            }
            if (!rm.RoleExists(evaRoles.instructor))
            {
                rm.Create(new IdentityRole(evaRoles.instructor));
            }
            return RedirectToAction("Index");
        }

        public ActionResult userToInstructor()
        {
            ViewBag.evaUserID = new SelectList(db.evaUsers, "ID", "fullName");
            return View(new instructorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userToInstructor([Bind(Include = "phoneNumber, alternativeMail")]instructorViewModel instructorVM ,string evaUserID)
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            evaUser user = db.evaUsers.Find(int.Parse(evaUserID));
            //check if the instructor already exists
            if (db.evaInstructor.Where(i => i.userName == user.userName).Count() >= 1)
            {
                //check if it has the ASPNET instructor role, add it if is not and the return
                var aspnetUser = userManager.FindByName(user.userName);
                if (!userManager.IsInRole(aspnetUser.Id, evaRoles.instructor))
                {
                    userManager.AddToRole(aspnetUser.Id, evaRoles.instructor);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("alternativeMail", "el usuario ya es un instructor");
                return View();
            }
            //if the instructor does not exist create it and add it to the database
            evaInstructor instructor = new evaInstructor()
            {
                userName = user.userName,
                email = user.email,
                altEmail = instructorVM.alternativeMail,
                fullName = user.fullName,
                mobileNumber = instructorVM.phoneNumber,
                aspnetUserID = user.aspnetUserID,
                gender = user.gender,
                isActive = true,
                evaOrganizationID = user.evaOrganizationID
            };
           
            db.evaInstructor.Add(instructor);
            db.SaveChanges();
            //add this user to the instructor role.
            var aspnetuser = userManager.FindByName(instructor.userName);
            userManager.AddToRole(aspnetuser.Id, evaRoles.instructor);

            return RedirectToAction("Index");
        }

        public ActionResult userToAdmin()
        {
            ViewBag.evaUserID = new SelectList(db.evaUsers, "ID", "fullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userToAdmin(string evaUserID)
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            evaUser user = db.evaUsers.Find(int.Parse(evaUserID));
            //add this user to the Admin role.
            var aspnetuser = userManager.FindByName(user.userName);
            userManager.AddToRole(aspnetuser.Id, evaRoles.Admin);

            return RedirectToAction("Index");
        }

        public ActionResult locatorTesting()
        {
            evaMediaServices mediaService = new evaMediaServices();
            mediaService.locatorStructure();
            return RedirectToAction("Index");
        }
        //use this actions to delete users, the main idea is to test this methods 
        //and finally create a Test of all the process of user creation
        public ActionResult userAdmin()
        {
            List<evaUser> users = db.evaUsers.ToList();
            return View(users);
        }
        public ActionResult deleteEvaUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaUser currentUser = db.evaUsers.Find(id);
            if (currentUser == null)
            {
                return HttpNotFound();
            }
            return View(currentUser);
        }
        [HttpPost, ActionName("deleteEvaUser")]
        [ValidateAntiForgeryToken]
        public ActionResult deleteEvaUserConfirmed(int id)
        {
            if (userUtils.deleteUserAndAspnetIdentity(id, db, new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()))))
            {
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            //if we get here the process failed somewhere
            return HttpNotFound();
        }

        //the actions below are used to get the user course and lessons detail info from the DB.
        public ActionResult userDataAdmin(int? evaUserID) {
            ViewBag.evaUserID = new SelectList(db.evaUsers, "ID", "fullName");
            if (evaUserID != null && Request.IsAjaxRequest())
            {
                List<evaCourseEnrollment> enrollments = db.evaUsers.Find(evaUserID).CourseEnrollments.ToList();
                return PartialView("_userInformation", enrollments);
            }
            return View(new List<evaCourseEnrollment>());
        }
        public ActionResult deleteEnrollment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaCourseEnrollment enrollment = db.evaCourseEnrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }
        [HttpPost, ActionName("deleteEnrollment")]
        [ValidateAntiForgeryToken]
        public ActionResult deleteEnrollmentConfirmed(int id)
        {
            evaCourseEnrollment enrollment = db.evaCourseEnrollments.Find(id);
            db.evaCourseEnrollments.Remove(enrollment);
            userUtils.decrementEnrolledCourses(db, enrollment.evaBaseUserID);
            db.SaveChanges();
            return RedirectToAction("userDataAdmin");
        }
        public ActionResult getUserLessonDetailInfo(int id)
        {
            //ID is a courseEnrollmentID
            if (Request.IsAjaxRequest())
            {
                List<evaLessonDetail> lessonDetail = db.evaCourseEnrollments.Find(id).lessonDetail.ToList();
                return PartialView("_userLessonDetail", lessonDetail);
            }
            return View(new List<evaLessonDetail>());
        }
        public ActionResult getUserQuestionDetailInfo(int id)
        {
            if (Request.IsAjaxRequest())
            {
                List<evaQuestionDetail> questionDetail = db.evaLessonDetails.Find(id).questionDetail.ToList();
                return PartialView("_userQuestionDetail", questionDetail);
            }
            return View(new List<evaQuestionDetail>());
        }
        public ActionResult getUserAnswerHistoryInfo(int id)
        {
            if (Request.IsAjaxRequest())
            {
                List<evaAnswerHistory> answerHistory = db.evaQuestionDetails.Find(id).answerHistory.ToList();
                return PartialView("_userAnswerHistory", answerHistory);
            }
            return View(new List<evaAnswerHistory>());
        }
    }
}