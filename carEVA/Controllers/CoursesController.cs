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
            //NEW MODEL INCLUDING COLABORATORS
            List<CourseProfileViewModel> courseProfiles = new List<CourseProfileViewModel>();
            int instructorID = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());

            if (organizationAreaID != null)
            {
                courseProfiles.Add(new CourseProfileViewModel
                {
                    myCourse = db.evaOrganizationAreas.Where(o => o.isExternal == true && o.evaOrganizationID == orgID)
                    .SelectMany(m => m.organizationCourses).Where(m => m.evaInstructorID == instructorID)
                    .Select(c => c.course),
                    organizationAreaID = organizationAreaID,
                    profileType = courseProfileTypes.externalCourse
                });
                return View(courseProfiles);
            }
            else
            {
                courseProfiles.Add(new CourseProfileViewModel
                {
                    myCourse = db.evaOrganizationAreas.Where(o => o.isExternal == false && o.evaOrganizationID == orgID)
                                    .SelectMany(m => m.organizationCourses).Where(m => m.evaInstructorID == instructorID)
                                    .Select(c => c.course),
                    profileType = courseProfileTypes.internalCourse
                });
                courseProfiles.Add(new CourseProfileViewModel
                {
                    myCourse = db.evaOrganizationAreas.Where(o => o.isExternal == true && o.evaOrganizationID == orgID)
                    .SelectMany(m => m.organizationCourses).Where(m => m.evaInstructorID == instructorID)
                    .Select(c => c.course),
                    organizationAreaID = organizationAreaID,
                    profileType = courseProfileTypes.externalCourse
                });
                //get the courses the colaborator has access to.
                List<evaOrganizationCourse> colaboratorOf = db.evaInstructor
                    .Where(m => m.ID == instructorID)
                    .SelectMany(m => m.colaboratorOf).ToList();
                List<evaOrganizationCourse> externalCourse = db.evaOrganizationAreas
                    .Where(o => o.isExternal == false)
                    .SelectMany(m => m.organizationCourses).ToList();
                courseProfiles.Add(new CourseProfileViewModel
                {
                    //myCourse = db.evaOrganizationAreas.Where(o => o.isExternal == false)
                    //.SelectMany(m => m.organizationCourses).Where(m => m.evaColaboratorID == instructorID)
                    //.Select(c => c.course),
                    //this selects all the internal courses the colaborator has access to
                    //myCourse = colaboratorOf
                    //    .Intersect(externalCourse, new evaOrganizationCourseComparer())
                    //    .Select(m => m.course).ToList(),

                    //this selects all the internal and external courses the colaborator has access to
                    myCourse = colaboratorOf.Select(m=>m.course).ToList(),
                    profileType = courseProfileTypes.sharedCourse
                });
            }

            return View(courseProfiles);
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
        public async Task<ActionResult> viewColaborators(int? courseID, int? organizationAreaID)
        {
            if (courseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            //Get the list of colaborators from the courseID to prevent addin a new viewmodel
            //plus by definition, a course can only be associated to an organization only ONE TIME.
            //TODO: as now there is no way to associate a course to multiple organizations.
            Course currentCourse = db.Courses.Find(courseID);
            ViewBag.currentCourse = currentCourse;
            evaOrganizationCourse orgCourse = currentCourse.organizationCourse
                .Where(m => m.evaOrganizationID == orgID).Single();
            ViewBag.organizationCourse = orgCourse;
            List<evaInstructor> colaborators = orgCourse.colaborators.ToList();

            return View(colaborators);
        }
        //add a colaborator to an organization course.
        public async Task<ActionResult> addColaborator(int? orgCourseID)
        {
            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            int currentInstructor = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
            if (orgCourseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //get the organization course from the course ID
            //By definition, a course can only be associated to an organization only ONE TIME.
            //evaOrganizationCourse orgCourse = db.evaOrganizationCourses
            //    .Include(m => m.colaborators)
            //    .SingleOrDefault(o => o.evaOrganizationCourseID == orgCourseID);
            evaOrganizationCourse orgCourse = db.evaOrganizationCourses.Find(orgCourseID);
            ViewBag.courseTitle = orgCourse.course.title;
            List<evaInstructor> instructores = db.evaInstructor
                .Where(i => i.evaOrganizationID == orgID && i.ID != currentInstructor).ToList();
            List<evaInstructor> colaboratos = orgCourse.colaborators.ToList();
            ViewBag.colaboratorID = new SelectList(instructores.Except(colaboratos, new evaBaseUserComparer())
                , "ID", "fullName");

            return View(orgCourse);
        }
        //add a colaborator to an organization course.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> addColaborator(int evaOrganizationCourseID, int colaboratorID)
        {
            evaOrganizationCourse orgCourse = db.evaOrganizationCourses.Find(evaOrganizationCourseID);
            evaInstructor colaborator = db.evaInstructor.Find(colaboratorID);
            orgCourse.colaborators.Add(colaborator);
            db.SaveChanges();

            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            int currentInstructor = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
            //redirect to the view
            return RedirectToAction("viewColaborators", new { courseID = orgCourse.course.CourseID/*, organizationAreaID = 1*/});
        }

        //remove colaborator
        // GET: Courses/removeColaborator?evaorganizationcourseId & colaboratorID
        //TODO: this action does not have forgery validation because its not a posted form
        public ActionResult removeColaborator(int evaOrganizationCourseID, int colaboratorID)
        {
            evaOrganizationCourse orgCourse = db.evaOrganizationCourses
                .Include(m => m.colaborators)
                .SingleOrDefault(m => m.evaOrganizationCourseID == evaOrganizationCourseID);
            evaInstructor colaborator = db.evaInstructor.Find(colaboratorID);
            //remove the given colaborator from the organization course
            orgCourse.colaborators.Remove(colaborator);
            db.SaveChanges();
            //TODO: design a way to handle the organization area filter
            return RedirectToAction("viewColaborators", new { courseID = orgCourse.course.CourseID/*, organizationAreaID = 1*/});
        }

        //create an external course
        // GET: Courses/Create
        public async Task<ActionResult> Create(int? organizationAreaID)
        {
            //populate the list of available areas for the course
            //what defines the course is external is the area of origin of the course
            int orgID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
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
            ViewBag.evaColaboratorID = new SelectList(db.evaInstructor
                .Where(m => m.evaOrganizationID == orgID), "ID", "fullName");
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
                    course.createdByID = await instructorUtils.instructorIdFromAsp(db, User.Identity.GetUserId(), userManager);
                    //some default values for the organization course
                    //by default the current logged user is the instructor of the course
                    orgCourse.evaInstructorID = course.createdByID;
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

                //now with the course ID we need to create the permission ACL.
                //as may 2018 we are enabling full access to all the CAR areas when created here
                List<evaOrgCourseAreaPermissions> permissions = new List<evaOrgCourseAreaPermissions>();
                List<evaOrganizationArea> orgAreas = db.evaOrganizations.Where(a => a.evaOrganizationID == orgCourse.evaOrganizationID)
                    .SelectMany(m => m.evaAreas).ToList();
                foreach (evaOrganizationArea item in orgAreas)
                {
                    permissions.Add(new evaOrgCourseAreaPermissions
                    {
                        evaOrganizationCourseID = orgCourse.evaOrganizationCourseID,
                        evaOrganizationAreaID = item.evaOrganizationAreaID,
                        permissionLevel = areaPermission.canEnrol
                    });
                }
                db.evaOrgCourseAreaPermissions.AddRange(permissions);
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

            //remove the assocualtes ACLs
            //call toList to populate the items
            foreach (evaOrganizationCourse item in course.organizationCourse.ToList())
            {
                //remove the permission table from the course, as it does not allow cascade delete
                db.evaOrgCourseAreaPermissions.RemoveRange(db.evaOrgCourseAreaPermissions
                    .Where(m => m.evaOrganizationCourseID == item.evaOrganizationCourseID));
            }
            

            //TODO: should i be deleting the organization course instead?
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
