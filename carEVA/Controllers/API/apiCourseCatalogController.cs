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
        private carEVAContext db = new carEVAContext();

        // GET: api/courseCatalog
        //TODO: add the [frombody] annotation to receive ths public key from the body of the request
        public IQueryable<evaOrganizationCourse> GetCourses(string publicKey)
        {
            //use this parameter if you dont want all the children to be populated
            
            db.Configuration.ProxyCreationEnabled = false;
            int orgID = userUtils.organizationIdFromKey(db, publicKey);
            int userId = userUtils.userIdFromKey(db, publicKey);

            //force the execution of the query here so the loops dont fail (deffered execution)
            var userEnrollments = db.evaCourseEnrollments.Where(p => p.evaUserID == userId).ToList();
            var orgCourses = db.evaOrganizationCourses.Where(p => p.evaOrganizationID == orgID).Include(m => m.course).ToList();

            foreach (evaOrganizationCourse item in orgCourses)
            {
                foreach(evaCourseEnrollment enrol in userEnrollments)
                {
                    if(enrol.CourseID == item.courseID)
                    {
                        enrol.evaUser = null;
                        //the user is enrolled in this course
                        item.course.enrollments.Add(enrol);
                    }
                }
            }

            return orgCourses.AsQueryable();
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
        public IHttpActionResult PostCourse([FromBody] userCourseEnrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (enrollment == null)
            {
                return BadRequest("no information received");
            }
            evaCourseEnrollment newEnrollment = new evaCourseEnrollment
            {
                evaUserID = userUtils.userIdFromKey(db, enrollment.publicKey),
                CourseID = enrollment.courseID,
                completedLessons = 0,
                EnrollmentDate = DateTime.Now
            };
            db.evaCourseEnrollments.Add(newEnrollment);
            db.SaveChanges();
            //db.Courses.Add(course);
            //db.SaveChanges();

            return StatusCode(HttpStatusCode.OK);
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