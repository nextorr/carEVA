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

using carEVA.ViewModels;
using carEVA.Utils;

namespace carEVA.Controllers.API
{
    public class courseCatalogController : ApiController
    {
        private carEVAContext db; 
        public courseCatalogController()
        {
            db=new carEVAContext();
        }

        public courseCatalogController( carEVAContext context)
        {
            db = context;
        }
        // GET: api/courseCatalog
        [ResponseType(typeof(evaOrganizationCourse))]
        public IHttpActionResult GetCourses(string publicKey)
        {
            int orgID;
            int userId;

            //use this parameter if you dont want all the children to be populated
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                orgID = userUtils.organizationIdFromKey(db, publicKey);
                userId = userUtils.userIdFromKey(db, publicKey);
            }
            catch (InvalidOperationException e)
            {
                //report the service client that the key they are using is invalid.
                evaLogUtils.logErrorMessage("invalid public Key",
                    publicKey, e, this.ToString(), nameof(this.GetCourses));
                return BadRequest("ERROR : 100, the public key is invalid");
            }

            //force the execution of the query here so the loops dont fail (deffered execution)
            var userEnrollments = db.evaCourseEnrollments.Where(p => p.evaBaseUserID == userId).ToList();
            var orgCourses = db.evaOrganizationCourses.Where(p => p.evaOrganizationID == orgID).Include(m => m.course).ToList();

            foreach (evaOrganizationCourse item in orgCourses)
            {
                foreach(evaCourseEnrollment enrol in userEnrollments)
                {
                    if(enrol.CourseID == item.courseID)
                    {
                        //this clears the user field so it is not sent on the response
                        enrol.evaUser = null;
                        //the user is enrolled in this course
                        //this writes the enrollment infor to be sent in the service
                        item.course.enrollments.Add(enrol);
                    }
                }
            }

            return Ok(orgCourses);
        }

        // GET: api/courseCatalog/5
        [ResponseType(typeof(Course))]
        public IHttpActionResult GetCourse(int id)
        {
            //use this parameter if you dont want all the children to be populated
            db.Configuration.ProxyCreationEnabled = false;
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT: api/courseCatalog/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.CourseID)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        /// <summary>
        /// this creates a new enrrollment for the given user and the given course
        /// </summary>
        /// <param name="enrollment">enrollment to commit</param>
        /// <returns></returns>
        // POST: api/courseCatalog
        [ResponseType(typeof(evaOrganizationCourse))]
        public IHttpActionResult PostCourse([FromBody] userCourseEnrollment enrollment)
        {
            return BadRequest("ERROR : 600, method not implemented");
            //TODO: marked to delete, enrollments are handled on the enrollment controller
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            //if (enrollment == null)
            //{
            //    return BadRequest("no information received");
            //}

            //int userID;
            //try
            //{
            //    userID = userUtils.userIdFromKey(db, enrollment.publicKey);
            //}
            //catch (InvalidOperationException e)
            //{
            //    //report the service client that the key they are using is invalid.
            //    evaLogUtils.logErrorMessage("invalid public Key",
            //        enrollment.publicKey, e.Message, this.ToString(), nameof(this.GetCourses));
            //    return BadRequest("ERROR : 100, the public key is invalid");
            //}

            //evaCourseEnrollment newEnrollment = new evaCourseEnrollment
            //{
            //    evaUserID = userID,
            //    CourseID = enrollment.courseID,
            //    completedLessons = 0,
            //    EnrollmentDate = DateTime.Now
            //};
            //db.evaCourseEnrollments.Add(newEnrollment);
            //db.SaveChanges();

            //return Created("DefaultApi", new evaResponses("enrolled succesfull", "OK"));
        }

        // DELETE: api/courseCatalog/5
        [ResponseType(typeof(Course))]
        public IHttpActionResult DeleteCourse(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Count(e => e.CourseID == id) > 0;
        }
    }
}