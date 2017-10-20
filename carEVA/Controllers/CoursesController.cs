using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;
using carEVA.ViewModels;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

using carEVA.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace carEVA.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private carEVAContext db = new carEVAContext();
        private ApplicationDbContext appContext;
        private UserManager<ApplicationUser> userManager;

        //initialization constructor
        public CoursesController()
        {
            appContext = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appContext));
        }
        // GET: Courses from the logged instructor
        [Authorize(Roles = evaRoles.Instructor)]
        public async Task<ActionResult> Index(int? organizationAreaID)
        {
            //use this to set up the view
            ViewBag.isExternal = false;
            int instructorID = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
            //USAGE: the course query depends on the parameters that are sent to the controller.
            //at the end we need to filter the courses to show only those of the logged in instructor
            List<Course> courses;
            IEnumerable<Course> courseQuery;
            if (organizationAreaID != null)
            {
                courseQuery = db.evaOrganizationAreas.Find(organizationAreaID).organizationCourses.Select(x => x.course);
                ViewBag.isExternal = db.evaOrganizationAreas.Find(organizationAreaID).isExternal;
                ViewBag.organizationAreaID = organizationAreaID;
            }
            else
            {
                //return only internal courses
                courseQuery = db.evaOrganizationAreas.Where(o => o.isExternal == false)
                    .SelectMany(m => m.organizationCourses).Select(c =>c.course);
            }
            courses = courseQuery.Where(c => c.evaInstructorID == instructorID).ToList();
            //construct the view model to see all the relevant data
            List<CoursesViewModels> courseVmList = new List<CoursesViewModels>();
            foreach (Course item in courses)
            {
                int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
                CoursesViewModels model = new CoursesViewModels()
                {
                    course = item,
                    organizationInfo = item.organizationCourse.Where(org => org.evaOrganizationID == orgID).FirstOrDefault()
                };
                courseVmList.Add(model);
            }
            return View(courseVmList);
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create(int? organizationAreaID)
        {
            //populate the list of available areas for the course
            //what defines the course is external is the area of origin of the course
            ViewBag.isExternal = false;
            evaOrganizationArea currentArea;
            if (organizationAreaID != null)
            {
                //as sept 2017 to create an external area the controller 
                //must be called witha organization areaID
                currentArea = db.evaOrganizationAreas.Find(organizationAreaID);
                ViewBag.isExternal = currentArea.isExternal;
                ViewBag.originAreaID = new SelectList(new evaOrganizationArea[] {currentArea}
                    , "evaOrganizationAreaID", "name", organizationAreaID);
            }
            else
            {
                currentArea = userUtils.areaFromAspIdentity(db, User.Identity.GetUserId());
                ViewBag.originAreaID = new SelectList(db.evaOrganizationAreas
                    , "evaOrganizationAreaID", "name", currentArea.evaOrganizationAreaID);
            }
            
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CourseID,title,description,commitmentDays,commitmentHoursPerDay", Prefix ="course")] Course course
            ,[Bind(Include = "evaOrganizationCourseID,creationDate,required,deadline, originAreaID", Prefix ="organizationInfo")] evaOrganizationCourse orgCourse)
        {
            if (ModelState.IsValid)
            {
                course.commitmentHoursTotal = course.commitmentDays * course.commitmentHoursPerDay;
                //its a new record, so we begin with a new state
                //important, this fields needs to be updated at runtime
                //when the user adds a quiz or a lesson.
                course.totalLessons = 0;
                course.totalQuizes = 0;
                course.totalPoints = 0;
                //TODO: this a default image, give the option lo load 
                //a specific image from the same form.
                course.evaImageID = 1;
                //associate the current logged user
                //and the organization course structure
                try
                {
                    course.evaInstructorID = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
                    //some default values for the organization course
                    orgCourse.evaOrganizationID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
                    course.organizationCourse = new List<evaOrganizationCourse>() {orgCourse};
                    db.Courses.Add(course);
                }
                catch (Exception e)
                {
                    evaLogUtils.logErrorMessage("invalid model", course, e, 
                   this.ToString(), nameof(this.Create));
                }
                //update the counters
                if (organizationUtils.incrementCourseCounter(db, orgCourse.evaOrganizationID, orgCourse.required) != 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error al encontrar la organizacion para actualizar contadores");
                }

                db.SaveChanges();

                //redirect back according to the external course definition
                if (!db.evaOrganizationAreas.Find(orgCourse.originAreaID).isExternal)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index", new { organizationAreaID = orgCourse.originAreaID });
            }
            else
            {
                evaLogUtils.logErrorMessage("invalid model",
                    this.ToString(), nameof(this.Create));
            }
            //we end up here if there are imvalid parameters or the binding fails
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Courses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CoursesViewModels courseViewModel = new CoursesViewModels();
            Course course = db.Courses.Find(id);
            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());

            if (course == null || course.organizationCourse == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            try
            {
                courseViewModel.course = course;
                courseViewModel.organizationInfo = course.organizationCourse.Where(m => m.evaOrganizationID == orgID).Single();
            }
            catch (Exception e)
            {
                string ErrorMessage = "Model incosistency, more than one orgCourse per Organization";
                evaLogUtils.logErrorMessage(ErrorMessage, course, e, this.ToString(), nameof(this.Edit));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessage);
            }

            //the areas dropdown list
            ViewBag.originAreaID = new SelectList(db.evaOrganizationAreas, "evaOrganizationAreaID", "name",
                    courseViewModel.organizationInfo.originAreaID);

            return View(courseViewModel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,title,description,commitmentDays,commitmentHoursPerDay,totalQuizes,totalLessons,evaImageID, evaInstructorID", Prefix ="course")] Course course
            ,[Bind(Include = "evaOrganizationCourseID, creationDate, originAreaID, required, deadline, evaOrganizationID, courseID", Prefix ="organizationInfo")] evaOrganizationCourse orgCourse)
        {
            //IMPORTANT NOTE: bind totalQuizes totalLessons evaImageID and any other fied that is not editable
            //this because if we dont bind then the model set this values as null in the database, and thats an incosistency
            if (ModelState.IsValid)
            {
                course.commitmentHoursTotal = course.commitmentDays * course.commitmentHoursPerDay;
                db.Entry(course).State = EntityState.Modified;
                db.Entry(orgCourse).State = EntityState.Modified;
                db.SaveChanges();

                if (!db.evaOrganizationAreas.Find(orgCourse.originAreaID).isExternal)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index", new { organizationAreaID = orgCourse.originAreaID });
            }
            //we end up here if there are imvalid parameters or the binding fails
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            try
            {
                //TODO: here is the organization of the logged user
                // 1 is the default for CAR.
                if (organizationUtils.decrementCourseCounter(db, 1, id) != 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error al encontrar la organizacion para reducir los contadores");
                }
            }
            catch (Exception)
            {
                //TODO: this is a inconsistency in the model, log this information and inform the user
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "the is an incosistency in the model, contact support CourseID: " + course.CourseID.ToString());
            }

            //TODO: take care of file deletion, cant use cascade delete because we need to sync 
            //the cloud blob storage
            var files = db.Files.Where(c => c.courseID == id);
            foreach (evaFile item in files)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                   ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("files");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(item.fileStorageName);

                try
                {
                    blockBlob.Delete();
                }
                catch (Exception)
                {
                    //if we cant delete the cloud file dont delete the database entry
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    throw;
                }
            }
            //take advantage of the fact that the FK are nullable, and that at minimum every entry has a courseID
            //so from here we can delete all files with the given courseID, and then we can assume the cascade delete
            //will delete chapters and lessons with no problem.
            //TODO: make sure that we always store the courseID, verify this in the creation of the entry in the file controller
            db.Files.RemoveRange(db.Files.Where(f => f.courseID == id));

            //the FK_dbo.evaOrganizationCourses_dbo.Courses_courseID has cascade delelte enabled
            //so deleting the course will delete the organization course.
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
