using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;

using carEVA.Utils;

namespace carEVA.Controllers
{
    public class ChaptersController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: Chapters
        //public ActionResult Index()
        //{
        //    var chapters = db.Chapters.Include(c => c.Course);
        //    ViewBag.viewType = "chapterAll";
        //    return View(chapters.ToList());
        //}

        // GET: Chapters
        public ActionResult Index(int? CourseID)
        {

            if (CourseID == null)
            {
                var chapters = db.Chapters.Include(c => c.Course);
                ViewBag.viewType = "chapterAll";
                return View(chapters.ToList());
            } else
            {
                var chapters = db.Chapters.Where(b => b.CourseID == CourseID).Include(c => c.Course);
                ViewBag.viewType = "chapterGroup";
                //var chapters = db.Chapters.Include(c => c.Course);
                if (!chapters.Any())
                {
                    //the dataset does not contain data.
                    //send a dummy chapter to get the parent info
                    List<Chapter> tempList = chapters.ToList();
                    tempList.Add(new Chapter() {
                        ChapterID = -1,
                        Course = db.Courses.Where(c => c.CourseID == CourseID).FirstOrDefault(),
                        CourseID = (int)CourseID});
                    return View(tempList);
                }
                return View(chapters.ToList());
            }
            
        }

        // GET: Chapters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        // GET: Chapters/Create
        public ActionResult Create(int? courseID)
        {
            if (courseID == null)
            {
                ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title");
            }
            else
            {
                ViewBag.CourseID = new SelectList(db.Courses.Where(b => b.CourseID == courseID), "CourseID", "title");
                ViewBag.backToID = courseID;
            }
            return View();
        }

        // POST: Chapters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChapterID,title,index,CourseID")] Chapter chapter, int? courseID)
        {
            if (ModelState.IsValid)
            {
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index", new { courseID = chapter.CourseID });
            }
            //now handle the course ID info
            if (courseID == null)
            {
                ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            }
            else
            {
                ViewBag.CourseID = new SelectList(db.Courses.Where(b => b.CourseID == courseID), "CourseID", "title", chapter.CourseID);
                ViewBag.backToID = courseID;
            }
            //ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            return View(chapter);
        }

        // GET: Chapters/Edit/5
        public ActionResult Edit(int? id, int? courseID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            //now handle the course ID info
            if (courseID == null)
            {
                ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            }
            else
            {
                ViewBag.CourseID = new SelectList(db.Courses.Where(b => b.CourseID == courseID), "CourseID", "title", chapter.CourseID);
                ViewBag.backToID = courseID;
            }
            //ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            return View(chapter);
        }

        // POST: Chapters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChapterID,title,index,CourseID")] Chapter chapter, int? courseID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {courseID = chapter.CourseID });
            }
            //now handle the course ID info
            if (courseID == null)
            {
                ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            }
            else
            {
                ViewBag.CourseID = new SelectList(db.Courses.Where(b => b.CourseID == courseID), "CourseID", "title", chapter.CourseID);
                ViewBag.backToID = courseID;
            }
            //ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "title", chapter.CourseID);
            return View(chapter);
        }

        // GET: Chapters/Delete/5
        public ActionResult Delete(int? id, int? courseID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            //now handle the course ID info
            if (courseID != null)
            {
                ViewBag.backToID = courseID;
            }
            return View(chapter);
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? courseID)
        {
            Chapter chapter = db.Chapters.Find(id);
            if(courseUtils.countDeletedLessons(db, chapter.CourseID, chapter) != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error al actualizar el contado eliminando un capitulo");
            }
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            if (courseID != null)
            {
                ViewBag.backToID = courseID;
                return RedirectToAction("Index", new {courseID = courseID });
            }
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
