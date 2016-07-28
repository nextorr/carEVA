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
    public class AnswersController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: Answers
        public ActionResult Index(int? questionID)
        {
            if(questionID == null)
            {
                var answers = db.Answers.Include(a => a.Question);
                ViewBag.viewType = "allItems";
                return View(answers.ToList());
            }
            else
            {
                var answers = db.Answers.Where(q => q.QuestionID == questionID)
                    .Include(a => a.Question.Lesson.Chapter.Course);
                ViewBag.viewType = "groupItems";
                if(!answers.Any())
                {
                    //the query does not return data, send a dummy object
                    List<Answer> tempList = answers.ToList();
                    tempList.Add(new Answer()
                    {
                        AnswerID = -1,
                        Question = db.Questions.Where(q => q.QuestionID == questionID)
                        .Include(l => l.Lesson.Chapter.Course).FirstOrDefault(),
                        QuestionID = (int)questionID
                    });
                    return View(tempList);
                }
                return View(answers.ToList());
            }
            
        }

        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create(int? questionID)
        {
            if (questionID == null)
            {
                ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement");
            }
            else
            {
                ViewBag.QuestionID = new SelectList(db.Questions.Where(q => q.QuestionID == questionID)
                    , "QuestionID", "statement");
                ViewBag.backToID = questionID;
            }
            
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnswerID,text,isCorrect,QuestionID")] Answer answer, int? questionID)
        {
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index", new { questionID = questionID });
            }
            //handle the questionID info
            if(questionID == null)
            {
                ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement", answer.QuestionID);
            }
            else
            {
                ViewBag.QuestionID = new SelectList(db.Questions.Where(q => q.QuestionID == questionID)
                    , "QuestionID", "statement", answer.QuestionID);
                ViewBag.backToID = questionID;
            }
            
            return View(answer);
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id, int? questionID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            //now handle the question ID
            if (questionID == null)
            {
                ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement", answer.QuestionID);
            }
            else
            {
                ViewBag.QuestionID = new SelectList(db.Questions.Where(q => q.QuestionID == questionID)
                    , "QuestionID", "statement", answer.QuestionID);
                ViewBag.backToID = questionID;
            }
            //ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement", answer.QuestionID);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnswerID,text,isCorrect,QuestionID")] Answer answer, int? questionID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {questionID = questionID });
            }
            //now handle the question ID
            if (questionID == null)
            {
                ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement", answer.QuestionID);
            }
            else
            {
                ViewBag.QuestionID = new SelectList(db.Questions.Where(q => q.QuestionID == questionID)
                    , "QuestionID", "statement", answer.QuestionID);
                ViewBag.backToID = questionID;
            }
            //ViewBag.QuestionID = new SelectList(db.Questions, "QuestionID", "statement", answer.QuestionID);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id, int? questionID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            //now handle the question ID
            if(questionID != null)
            {
                ViewBag.backToID = questionID;
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? questionID)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();
            //now handle the question ID
            if (questionID != null)
            {
                ViewBag.backToID = questionID;
                return RedirectToAction("Index", new {questionID = questionID });
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
