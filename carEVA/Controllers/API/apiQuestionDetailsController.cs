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
        //[ResponseType(typeof(IQueryable<userQuiz>))]
        [ResponseType(typeof(userQuizDetail))]
        public IHttpActionResult GetevaQuestionDetails(string publicKey, int lessonDetailID)
        {
            db.Configuration.ProxyCreationEnabled = false;

            if (lessonDetailID < 0)
            {
                evaLogUtils.logWarningMessage("invalid lessonDetailID",
                    this.ToString(), nameof(this.GetevaQuestionDetails));
                return BadRequest("ERROR : invalid parameters");
            }

            evaLessonDetail currentLessonDetail;
            int currentUser;
            try
            {
                //validate the public key
                //this is slightly different from the method used in apiGrader controller, check which is best
                currentLessonDetail = db.evaLessonDetails.Where(p => p.evaLessonDetailID == lessonDetailID)
                    .Include(m => m.questionDetail).Include(m => m.courseEnrollment).Single();
                currentUser = userUtils.userIdFromKey(db, publicKey);
                if (currentLessonDetail.courseEnrollment.evaBaseUserID != currentUser)
                {
                    evaLogUtils.logWarningMessage("invalid publick key for the evaluation",
                        this.ToString(), nameof(this.GetevaQuestionDetails));
                    return BadRequest("ERROR : la clave publica no es valida para esta evaluacion");
                }
            }
            catch (InvalidOperationException e)
            {
                //report the service client that the key they are using is invalid.
                evaLogUtils.logErrorMessage("invalid public Key",
                    publicKey, e, this.ToString(), nameof(this.GetevaQuestionDetails));
                return BadRequest("ERROR : 100, the public key is invalid");
            }

            
            //TODO: must also return the question info, create a viewmodel for that.
            List<userQuiz> quizDetailList = new List<userQuiz>();

            userQuizDetail response = new userQuizDetail();
            response.viewed = currentLessonDetail.viewed;
            response.passed = currentLessonDetail.passed;
            response.totalObtainedPoints = currentLessonDetail.currentTotalGrade;

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
                catch (InvalidOperationException e)
                {
                    evaLogUtils.logErrorMessage("model incosistency getting current detail",
                    publicKey, e, this.ToString(), nameof(this.GetevaQuestionDetails));
                    if(detailList.Count() > 1)
                    {
                        return BadRequest("ERROR : inconsistencia en el modelo, existe mas de un detalle para una pregunta");
                    }
                    //return BadRequest("ERROR : inconsistencia en el modelo, no hay detalle para la pregunta");
                }
                //erase the values for the responses on the answer so its not possible to view the responses
                questionItem.answerOptions.Select(c => { c.isCorrect = false; return c; }).ToList();
                //when the user first opens the quiz, we expect the detail to be empty, since single() fails, null is sent
                quizDetailList.Add(new userQuiz()
                {
                    question = questionItem,
                    detail = currentDetail
                });
            }

            response.quizDetail = quizDetailList;

            //return Ok(response.AsQueryable());
            return Ok(response);

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