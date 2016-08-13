using carEVA.Models;
using carEVA.Utils;
using carEVA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Data.Entity;
using System.Web.Http.Description;

namespace carEVA.Controllers.API
{
    public class graderController : ApiController
    {
        private carEVAContext db = new carEVAContext();
        // GET: api/grader
        [ResponseType(typeof(evaLessonDetail))]
        public IHttpActionResult Getgrader(string publicKey, quizResponses quizResponse)
        {
            db.Configuration.ProxyCreationEnabled = false;
            evaLessonDetail currentLessonDetail = db.evaLessonDetails.Find(quizResponse.lessonDetailID);
            //validate the publickey
            int currentUser = userUtils.userIdFromKey(db, publicKey);
            if(currentLessonDetail.courseEnrollment.evaUserID != currentUser)
            {
                return BadRequest("ERROR: la clave publica no es valida para esta evaluacion");
            }

            //query the list of questions and its detail for the given lesson
            var questions = db.Questions.Where(p => p.LessonID == currentLessonDetail.lessonID)
                .Include(m => m.answerOptions).ToList();
            var questionDetail = currentLessonDetail.questionDetail.ToList();
            int score = 0;
            int totalLessonPoints = 0;
            foreach(response responseItem in quizResponse.responses)
            {
                //evaluatino parameters, initialize as the current response is correct
                float pointsForAnswer = 0;
                int grongAttempt = 0;
                bool correct = true;
                int reduceMaxScoreWeight = 0;
                //get the question info
                Question questionInfo = questions.Where(q => q.QuestionID == responseItem.questionID).Single();
                totalLessonPoints = totalLessonPoints + questionInfo.points;
                //then, check if there is question detail
                if (questionDetail.Where(q => q.questionID == responseItem.questionID).Count() == 0)
                {
                    //there is no detail, so its the first grade attempt for this question
                    if (questionInfo.answerOptions.Where(q => q.AnswerID == responseItem.answerID).Single().isCorrect)
                    {
                        //the selected anwers for this question is correct
                        pointsForAnswer = questionInfo.points;
                        score = score + (int)pointsForAnswer;
                    }
                    else
                    {
                        //grong Answer! no points :(
                        pointsForAnswer = 0;
                        grongAttempt = 1;
                        correct = false;
                        //TODO IMPORTANT!!: this defines the evaluation logic. test diferent values.
                        reduceMaxScoreWeight = 10;
                    }
                    //create a history item
                    evaAnswerHistory historyItem = new evaAnswerHistory()
                    {
                        submitedDate = DateTime.Now,
                        selectedAnswerID = responseItem.answerID,
                        maxScore = 100 - reduceMaxScoreWeight, //percent value
                        isCorrect = correct,
                        score = (int)Math.Ceiling(pointsForAnswer)
                    };
                    //then create and store a question detail
                    evaQuestionDetail newQuestionDetail = new evaQuestionDetail()
                    {
                        evaLessonDetailID = quizResponse.lessonDetailID,
                        questionID = responseItem.questionID,
                        lastGradedAnswerID = responseItem.questionID,
                        isCorrect = correct,
                        totalGrongAttempts = grongAttempt,
                        finalScore = (int)Math.Ceiling(pointsForAnswer),
                        currentMaxScore = 100 - reduceMaxScoreWeight, // this is the evaluation weight. it reduces for every wrong attempt
                    };
                    newQuestionDetail.answerHistory.Add(historyItem);
                    //TODO: WARNING!! test that saving just this entity also saves the history item
                    db.evaQuestionDetails.Add(newQuestionDetail);
                    db.SaveChanges();
                }
                else if (questionDetail.Where(q => q.questionID == responseItem.questionID).Count() == 1)
                {
                    //there exist a question detail for this answer.
                    evaQuestionDetail qDetails = questionDetail.Where(q => q.questionID == responseItem.questionID).Single();
                    if(questionInfo.answerOptions.Where(q => q.AnswerID == responseItem.answerID).Single().isCorrect)
                    {
                        //the selected answer for this question is correct
                        pointsForAnswer = (questionInfo.points * qDetails.currentMaxScore)/100;
                        score = score + (int)Math.Ceiling(pointsForAnswer); //round Up to the highest number
                    }
                    else
                    {
                        //grong answer, no points :(
                        pointsForAnswer = 0;
                        grongAttempt = 1;
                        correct = false;
                        //TODO IMPORTANT!!: this defines the evaluation logic. test diferent values.
                        reduceMaxScoreWeight = 10;
                    }
                    //create a history item
                    evaAnswerHistory historyItem = new evaAnswerHistory()
                    {
                        submitedDate = DateTime.Now,
                        selectedAnswerID = responseItem.answerID,
                        maxScore = qDetails.currentMaxScore - reduceMaxScoreWeight, //percent value
                        isCorrect = correct,
                        score = (int)Math.Ceiling(pointsForAnswer)
                    };

                    //update the current question detail. 
                    //TODO: what IF the previously graded answer was correct
                    //for now, we just overwrite the result
                    qDetails.lastGradedAnswerID = responseItem.answerID;
                    qDetails.isCorrect = correct;
                    qDetails.totalGrongAttempts = qDetails.totalGrongAttempts + grongAttempt;
                    qDetails.finalScore = (int)Math.Ceiling(pointsForAnswer);
                    qDetails.currentMaxScore = qDetails.currentMaxScore - reduceMaxScoreWeight;

                    db.Entry(qDetails).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //model inconsistency, there is more than one detail for the answer for the given user.
                    return BadRequest("ERROR: inconsistencia, se encontro mas de 1 detalle para la despuesta");
                }

            }
            //grade the entire quiz to 60% of total points minimum to pass
            if (score >= (totalLessonPoints * 0.6))
            {
                //passed
                currentLessonDetail.viewed = true;
                currentLessonDetail.passed = true;
                currentLessonDetail.currentTotalGrade = score;
                currentLessonDetail.completionDate = DateTime.Now;
            }
            else
            {
                //not passed :(
                currentLessonDetail.viewed = true;
                currentLessonDetail.passed = false;
                currentLessonDetail.currentTotalGrade = score;
            }
            db.Entry(currentLessonDetail).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(currentLessonDetail);
        }

        // GET: api/grader/5
        public string Getgrader(int id)
        {
            return "value";
        }

        // POST: api/grader
        public void Postgrader([FromBody]string value)
        {
            //TODO: evaluate if the grading proces is better placed on the post method
        }

        // PUT: api/grader/5
        public void Putgrader(int id, [FromBody]string value)
        {
        }

        // DELETE: api/grader/5
        public void Deletegrader(int id)
        {
        }
    }
}
