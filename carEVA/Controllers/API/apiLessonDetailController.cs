﻿using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using carEVA.ViewModels;
using System.Web.Http.Description;
using System.Web.Http;
using carEVA.Utils;
using System.Data.Entity;

namespace carEVA.Controllers.API
{
    public class lessonDetailController : ApiController
    {
        private carEVAContext db = new carEVAContext();
        // GET: lessonDetail
        [ResponseType(typeof(IQueryable<userChapterDetail>))]
        public IHttpActionResult GetlessonDetails(int courseID, string publicKey)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<userChapterDetail> response = new List<userChapterDetail>();

            if (courseID < 0)
            {
                return BadRequest("ERROR 200 : invalid parameters");
            }

            int currentUserID;

            try
            {
               currentUserID = userUtils.userIdFromKey(db, publicKey);
            }
            catch (InvalidOperationException e)
            {
                //report the service client that the key they are using is invalid.
                evaLogUtils.logErrorMessage("invalid public Key",
                    publicKey, e, this.ToString(), nameof(this.GetlessonDetails));
                return BadRequest("ERROR : 100, the public key is invalid");
            }

            //get the chapter and lessons list for the given course
            var chapterList = db.Chapters.Where(m => m.CourseID == courseID).Include(m => m.lessons).ToList();

            evaCourseEnrollment enrollment;
            try
            {
                enrollment = db.evaCourseEnrollments.Where(m => m.CourseID == courseID && m.evaBaseUserID == currentUserID)
                .Include(m => m.lessonDetail).Single();
            }
            catch (InvalidOperationException e)
            {
                evaLogUtils.logErrorMessage("user not enrolled or enrolled more than one time",
                    publicKey, e, this.ToString(), nameof(this.GetlessonDetails));
                return BadRequest("ERROR : el usuario no esta inscrito o esta inscrito mas de una vez al curso");
            }
            if(chapterList == null)
            {
                //then log the information
                evaLogUtils.logWarningMessage("no lessons for the selected course " + courseID.ToString(),
                     this.ToString(), nameof(this.GetlessonDetails));
                //return an empty response
                return Ok(new userChapterDetail());
                //return BadRequest("ERROR : no hay lecciones aun para el curso seleccionado");
            }
            
            //merge de chapterinfo (title..etc) with the user detail (completed ones etc..)
            userChapterDetail itemBuilder;
            userLessonDetail detailBuilder;
            evaLessonDetail userDetail;
            //flag to allow bulk insertion.
            bool dataToSave = false;
            foreach(Chapter chapterItem in chapterList)
            {
                itemBuilder = new userChapterDetail() {
                    chapter = chapterItem,
                    percentViewed = 0,
                    lessons = new List<userLessonDetail>()};
                foreach(Lesson lessonItem in chapterItem.lessons)
                {
                    //we could query the enrollment.lessonsDetail with the lesson ID for the given chapter, 
                    //but we do the double loop to take into account details that does not exist for a given lesson
                    int totalLessonsInChapter = chapterItem.lessons.Count();
                    detailBuilder = new userLessonDetail() {info = lessonItem };
                    var detail = enrollment.lessonDetail.Where(l => l.lessonID == lessonItem.LessonID);
                    if(detail != null)
                    {
                        if(detail.Count() == 1)
                        {
                            detailBuilder.userDetail = detail.Single();
                            if (detailBuilder.userDetail.viewed)
                            {
                                itemBuilder.percentViewed = itemBuilder.percentViewed + (100 / totalLessonsInChapter);
                            }
                        }
                        if(detail.Count() == 0)
                        {
                            //there is no detail stored in this lesson for the given user.
                            //create the detail in the database
                            //LG: creating the detail requires this loop, so, seems fine
                            //if we create the detail the first time the course is listed
                            //instead of when the user enrolls in the course.
                            userDetail = new evaLessonDetail() {
                                evaCourseEnrollmentID = enrollment.evaCourseEnrollmentID,
                                lessonID = lessonItem.LessonID,
                                viewed = false,
                                currentTotalGrade = 0,
                                completionDate = null
                            };
                            detailBuilder.userDetail = userDetail;
                            //persist this detail in the database
                            db.evaLessonDetails.Add(userDetail);
                            dataToSave = true;
                        }
                    }
                    itemBuilder.lessons.Add(detailBuilder);
                }
                response.Add(itemBuilder);
            }

            //save changes if there is something to ADD to the database
            if(dataToSave)
                db.SaveChanges();

            response = response.OrderBy(p => p.chapter.index).ToList();
            //clean some fileds before storing the result to avoid redundant data to be sent
            //IMPORTANT: do response modifications here, after the data is saved in the database.
            //to avoid model invalidation errors
            foreach (userChapterDetail item in response)
            {
                item.chapter.lessons = null;
                foreach (userLessonDetail detailItem in item.lessons)
                {
                    detailItem.userDetail.courseEnrollment = null;
                    detailItem.info.Chapter = null;
                }
            }

            return Ok(response);
        }

        // GET: api/lessonDetail/5
        public string GetlessonDetail(int id)
        {
            return "value";
        }

        // POST: api/lessonDetail
        public void PostlessonDetail([FromBody]string value)
        {
        }

        // PUT: api/lessonDetail/5
        public void PutlessonDetail(int id, [FromBody]string value)
        {
        }

        // DELETE: api/lessonDetail/5
        public void DeletelessonDetail(int id)
        {
        }
    }
}
