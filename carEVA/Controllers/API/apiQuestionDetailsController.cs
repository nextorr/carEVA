using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using carEVA.Models;
using carEVA.Utils;
using carEVA.ViewModels;

namespace carEVA.Controllers.API
{
    public class questionDetailsController : ApiController
    {
        private carEVAContext db = new carEVAContext();
        // GET: api/questiondetails
        [ResponseType(typeof(IQueryable<userQuiz>))]
        public IHttpActionResult GetevaQuestionDetails(string publicKey, int lessonDetailID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //validate the public key
            //this is slightly different from the method used in apiGrader controller, check which is best
            evaLessonDetail currentLessonDetail = db.evaLessonDetails.Where(p => p.evaLessonDetailID == lessonDetailID)
                .Include(m => m.questionDetail).Include(m => m.courseEnrollment).Single();
            int currentUser = userUtils.userIdFromKey(db, publicKey);
            if (currentLessonDetail.courseEnrollment.evaUserID != currentUser)
            {
                return BadRequest("ERROR: la clave publica no es valida para esta evaluacion");
            }
            //TODO: must also return the question info, create a viewmodel for that.
            List<userQuiz> response = new List<userQuiz>();
            var lessonQuestions = db.Questions.Where(p => p.LessonID == currentLessonDetail.lessonID).Include(m => m.answerOptions);
            foreach(Question questionItem in lessonQuestions)
            {
                //match the current question with its detail to build the response
                var detailList = currentLessonDetail.questionDetail.Where(q => q.questionID == questionItem.QuestionID);
                evaQuestionDetail currentDetail = null;

                try
                {
                    currentDetail = detailList.Single();
                }
                catch (InvalidOperationException)
                {
                    if(detailList.Count() > 1)
                    {
                        return BadRequest("ERROR: inconsistencia en el modelo, existe mas de un detalle para una pregunta");
                    }
                }
                //when the user first opens the quiz, we expect the detail to be empty, since single() fails, null is sent
                response.Add(new userQuiz()
                {
                    question = questionItem,
                    detail = currentDetail
                });
            }
            return Ok(response.AsQueryable());
        }

        // GET: api/questiondetails/5
        [ResponseType(typeof(evaQuestionDetail))]
        public IHttpActionResult GetevaQuestionDetail(int id)
        {
            evaQuestionDetail evaQuestionDetail = db.evaQuestionDetails.Find(id);
            if (evaQuestionDetail == null)
            {
                return NotFound();
            }

            return Ok(evaQuestionDetail);
        }

        // PUT: api/questiondetails/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutevaQuestionDetail(int id, evaQuestionDetail evaQuestionDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaQuestionDetail.evaQuestionDetailID)
            {
                return BadRequest();
            }

            db.Entry(evaQuestionDetail).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!evaQuestionDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/questiondetails
        [ResponseType(typeof(evaQuestionDetail))]
        public IHttpActionResult PostevaQuestionDetail(evaQuestionDetail evaQuestionDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.evaQuestionDetails.Add(evaQuestionDetail);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = evaQuestionDetail.evaQuestionDetailID }, evaQuestionDetail);
        }

        // DELETE: api/questiondetails/5
        [ResponseType(typeof(evaQuestionDetail))]
        public IHttpActionResult DeleteevaQuestionDetail(int id)
        {
            evaQuestionDetail evaQuestionDetail = db.evaQuestionDetails.Find(id);
            if (evaQuestionDetail == null)
            {
                return NotFound();
            }

            db.evaQuestionDetails.Remove(evaQuestionDetail);
            db.SaveChanges();

            return Ok(evaQuestionDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool evaQuestionDetailExists(int id)
        {
            return db.evaQuestionDetails.Count(e => e.evaQuestionDetailID == id) > 0;
        }
    }
}