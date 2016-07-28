using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;

namespace carEVA.Controllers
{
    public class QuestionsController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: Questions
        public ActionResult Index(int? lessonID)
        {
            if(lessonID == null)
            {
                var questions = db.Questions.Include(q => q.Lesson);
                ViewBag.viewType = "allItems";
                return View(questions.ToList());
            }
            else
            {
                var questions = db.Questions.Where(l => l.LessonID == lessonID).Include(q => q.Lesson.Chapter.Course);
                ViewBag.viewType = "groupItems";
                if (!questions.Any())
                {
                    //the query returned no data, send a dummy object to the view
                    List<Question> tempList = questions.ToList();
                    tempList.Add(new Question()
                    {
                        QuestionID = -1,
                        Lesson = db.Lessons.Where(l => l.LessonID == lessonID)
                        .Include(c => c.Chapter.Course).FirstOrDefault(),
                        LessonID = (int)lessonID
                    });
                    return View(tempList);
                }
                return View(questions.ToList());
            }
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create(int? lessonID)
        {
            if(lessonID == null)
            {
                ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title");
                ViewBag.evaType = new SelectList(db.evaTypes, "evaTypeID", "answerType");
            }
            else
            {
                ViewBag.LessonID = new SelectList(db.Lessons.Where(l => l.LessonID == lessonID), "LessonID", "title");
                ViewBag.evaType = new SelectList(db.evaTypes, "evaTypeID", "answerType");
                ViewBag.backToID = lessonID;
            }
            
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionID,statement,evaType,LessonID")] Question question, int? lessonID)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index", new { lessonID = lessonID });
            }
            //handle the lessonID info
            if (lessonID == null)
            {
                ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            }
            else
            {
                ViewBag.LessonID = new SelectList(db.Lessons.Where(l => l.LessonID == lessonID)
                    , "LessonID", "title", question.LessonID);
                ViewBag.backToID = lessonID;
            }
            //ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id, int? lessonID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            //now handle the lessonID
            if (lessonID == null)
            {
                ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            }
            else
            {
                ViewBag.LessonID = new SelectList(db.Lessons.Where(l => l.LessonID == lessonID), "LessonID", "title", question.LessonID);
                ViewBag.backToID = lessonID;
            }
            //ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionID,statement,evaType,LessonID")] Question question, int? lessonID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {lessonID = lessonID });
            }
            //now handle the lessonID
            if (lessonID == null)
            {
                ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            }
            else
            {
                ViewBag.LessonID = new SelectList(db.Lessons.Where(l => l.LessonID == lessonID), "LessonID", "title", question.LessonID);
                ViewBag.backToID = lessonID;
            }
            //ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "title", question.LessonID);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id, int? lessonID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            //noew handle the lessonID
            if(lessonID != null)
            {
                ViewBag.backToID = lessonID;
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? lessonID)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            //noew handle the lessonID
            if (lessonID != null)
            {
                ViewBag.backToID = lessonID;
                return RedirectToAction("Index", new { lessonID = lessonID });
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
