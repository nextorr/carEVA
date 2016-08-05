using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace carEVA.Controllers
{
    public class CoursesController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: Courses
        public ActionResult Index()
        {
            db.evaOrganizationCourses.Where(oc => oc.evaOrganizationID == 1).Include(b=>b.course);
            return View(db.Courses.ToList());
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,title,description, commitmentDays, commitmentHoursPerDay")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.commitmentHoursTotal = course.commitmentDays * (int)course.commitmentHoursPerDay;
                //its a new record, so we begin with a new state
                //important, this fields needs to be updated at runtime
                //when the user adds a quiz or a lesson.
                course.totalLessons = 0;
                course.totalQuizes = 0;
                //TODO: this a default image, give the option lo load 
                //a specific image from the same form.
                course.evaImageID = 1;

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //TODO: investigate a way of returnig an error message
            //IE the case where the controller receives null numbers
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Course course = db.Courses.Find(id);
            Course course = db.Courses.Include(i => i.Chapters).Where(i => i.CourseID == id).Single();
            evaOrganizationCourse organizationCourse;
            Chapter chapter = course.Chapters.Where(c => c.ChapterID == 1).Single();
            var query = db.evaOrganizationCourses.Where(s => s.evaOrganizationID == 1 && s.courseID == course.CourseID);
            course.organization = query.ToList();
            try
            {
                organizationCourse = query.Single();
            }
            catch (System.InvalidOperationException)
            {
                if (query.Count() != 0)
                {
                    //if we end up here the query return more tha one resutl
                    //since this is a mayor incosistency throw an exception
                    throw (new Exception("Data model inconsistency, contact support for more information"));
                }
                else
                {
                    //if the result is empty explicitly make organization course null
                    organizationCourse = null;
                }
            }

            if (organizationCourse == null)
            {
                //empty organizationCourse association.
                //this is only valid on development
                //TODO: in production log an error here as this is an incosistency in the data model
                ViewBag.areaNameID = new SelectList(db.evaOrganizationAreas, "evaOrganizationAreaID", "name");
            }
            else
            {
                ViewBag.areaNameID = new SelectList(db.evaOrganizationAreas, "evaOrganizationAreaID", "name",
                    organizationCourse.originAreaID);
            }
            
            
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "CourseID,title,description,commitmentDays,commitmentHoursPerDay,totalQuizes,totalLessons,evaImageID")] Course course, string areaNameID)
        {
            if (ModelState.IsValid)
            {
                course.commitmentHoursTotal = course.commitmentDays * course.commitmentHoursPerDay;
                //the evaOrganizationID comes from the logged user information
                evaOrganizationCourse payload = db.evaOrganizationCourses.Single(s => s.evaOrganizationID == 1 && s.courseID == course.CourseID);
                payload.originAreaID = int.Parse(areaNameID);
                db.Entry(course).State = EntityState.Modified;
                db.Entry(payload).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
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
