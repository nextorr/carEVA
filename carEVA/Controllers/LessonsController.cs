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
    public class LessonsController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: Lessons
        public ActionResult Index(int? chapterID)
        {
            if(chapterID == null)
            {
                var lessons = db.Lessons.Include(l => l.Chapter);
                ViewBag.viewType = "lessonAll";
                return View(lessons.ToList());
            }
            else
            {
                var lessons = db.Lessons.Where(c => c.ChapterID == chapterID).Include(l => l.Chapter.Course);
                ViewBag.viewType = "lessonGroup";
                if (!lessons.Any())
                {
                    //the returned query does not contain data, send a dummy object to the view
                    List<Lesson> tempList = lessons.ToList();
                    tempList.Add(new Lesson
                    {
                        LessonID = -1,
                        Chapter = db.Chapters.Where(c => c.ChapterID == chapterID)
                        .Include(cap => cap.Course).FirstOrDefault(),
                        ChapterID = (int)chapterID
                    });
                    return View(tempList);
                }
                return View(lessons.ToList());
            }
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {   
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create(int? chapterID)
        {
            if(chapterID == null)
            {
                ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title");
            }
            else
            {
                ViewBag.ChapterID = new SelectList(db.Chapters.Where(c => c.ChapterID == chapterID), "ChapterID", "title");
                ViewBag.backToID = chapterID;
            }
            //ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonID,title,description,videoURL,ChapterID")] Lesson lesson, int? chapterID)
        {
            if (ModelState.IsValid)
            {
                int courseID = db.Chapters.Find(lesson.ChapterID).CourseID;
                if(courseUtils.incrementTotalLessons(db, courseID) != 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Course not found");
                }
                //succesfully added one to the course model, persist the data
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index", new { chapterID = lesson.ChapterID});
            }
            //now handle the chapter ID info
            if (chapterID == null)
            {
                ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title", lesson.ChapterID);
            }
            else
            {
                ViewBag.ChapterID = new SelectList(db.Chapters.Where(c => c.ChapterID == chapterID), "ChapterID", "title", lesson.ChapterID);
                ViewBag.backToID = chapterID;
            }
            //ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title", lesson.ChapterID);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id, int? chapterID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            //now handle the chapter ID info
            if (chapterID == null)
            {
                ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title", lesson.ChapterID);
            }
            else
            {
                ViewBag.ChapterID = new SelectList(db.Chapters.Where(c => c.ChapterID == chapterID), "ChapterID", "title", lesson.ChapterID);
                ViewBag.backToID = chapterID;
            }
            //ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title", lesson.ChapterID);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonID,title,description,videoURL,ChapterID")] Lesson lesson, int? chapterID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {chapterID = lesson.ChapterID });
            }
            //now handle the chapter ID info
            if (chapterID == null)
            {
                ViewBag.ChapterID = new SelectList(db.Chapters, "ChapterID", "title", lesson.ChapterID);
            }
            else
            {
                ViewBag.ChapterID = new SelectList(db.Chapters.Where(c => c.ChapterID == chapterID), "ChapterID", "title", lesson.ChapterID);
                ViewBag.backToID = chapterID;
            }
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id, int? chapterID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            //now handle the chapter ID info
            if (chapterID != null)
            {
                ViewBag.backToID = chapterID;
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? chapterID)
        {
            Lesson lesson = db.Lessons.Find(id);
            int courseID = db.Chapters.Find(lesson.ChapterID).CourseID;
            if (courseUtils.decrementTotalLessons(db, courseID, lesson) != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "curso no encontrado al tratar de actualizar los contadores");
            }
            db.Lessons.Remove(lesson);
            db.SaveChanges();
            //now handle the chapter ID info
            if (chapterID != null)
            {
                ViewBag.backToID = chapterID;
                return RedirectToAction("Index", new {chapterID = chapterID });
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
