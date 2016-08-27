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
using System.Web.Mvc;

namespace carEVA.Controllers
{
    public class courseEnrollmentsController : ApiController
    {
        private carEVAContext db = new carEVAContext();

        // GET: api/courseEnrollments
        public IQueryable<userEnrollmets> GetcourseEnrollments(string publicKey)
        {
            //use this parameter if you dont want all the children to be populated
            List<userEnrollmets> response = new List<userEnrollmets>();
            db.Configuration.ProxyCreationEnabled = false;
            int userId = userUtils.userIdFromKey(db, publicKey);
            int userCompany = userUtils.organizationIdFromKey(db, publicKey);
            var enrollments = db.evaCourseEnrollments.Where(m => m.evaUserID == userId).Include(c => c.Course).ToList();
            var companyParameters = db.evaOrganizationCourses.Where(m => m.evaOrganizationID == userCompany).ToList();

            foreach(evaCourseEnrollment enrollmentItem in enrollments)
            {
                foreach(evaOrganizationCourse orgCourseItem in companyParameters)
                {
                    //we need to send in the response if the course is required, so we buld our view model
                    // NOTE removed  && orgCourseItem.required
                    if ((enrollmentItem.CourseID == orgCourseItem.courseID))
                    {
                        //since we are bulding the reponse from scratch, remove the user info since we dont need it
                        enrollmentItem.evaUser = null;
                        enrollmentItem.Course.organizationCourse = null;
                        if (orgCourseItem.required)
                            response.Add(new userEnrollmets() { dueDate = orgCourseItem.deadline, enrollment = enrollmentItem });
                        else
                            response.Add(new userEnrollmets() { dueDate = null, enrollment = enrollmentItem });
                    }
                }
            }

            return response.AsQueryable();
        }

        // GET: api/courseEnrollments/5
        [ResponseType(typeof(evaCourseEnrollment))]
        public IHttpActionResult GetevaCourseEnrollment(int id)
        {
            evaCourseEnrollment evaCourseEnrollment = db.evaCourseEnrollments.Find(id);
            if (evaCourseEnrollment == null)
            {
                return NotFound();
            }

            return Ok(evaCourseEnrollment);
        }

        // PUT: api/courseEnrollments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutevaCourseEnrollment(int id, evaCourseEnrollment evaCourseEnrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaCourseEnrollment.evaCourseEnrollmentID)
            {
                return BadRequest();
            }

            db.Entry(evaCourseEnrollment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!evaCourseEnrollmentExists(id))
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

        // POST: api/courseEnrollments
        [ResponseType(typeof(evaCourseEnrollment))]
        public IHttpActionResult PostcourseEnrollment([FromBody] userCourseEnrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (enrollment == null && enrollment.publicKey == null && enrollment.courseID <= 0)
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
            if (userUtils.incrementEnrolledCourses(db, enrollment.publicKey) != 1)
            {
                //this just checks that we made the increment at entity level, 
                //this is not saved to database until we call savechanges
                return BadRequest("Error al actualizar los contadores del usuario");
            }
            db.evaCourseEnrollments.Add(newEnrollment);
            db.SaveChanges();

            return Ok(newEnrollment);
        }

        // DELETE: api/courseEnrollments/5
        [ResponseType(typeof(evaCourseEnrollment))]
        public IHttpActionResult DeleteevaCourseEnrollment(int id)
        {
            evaCourseEnrollment evaCourseEnrollment = db.evaCourseEnrollments.Find(id);
            if (evaCourseEnrollment == null)
            {
                return NotFound();
            }

            db.evaCourseEnrollments.Remove(evaCourseEnrollment);
            db.SaveChanges();

            return Ok(evaCourseEnrollment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool evaCourseEnrollmentExists(int id)
        {
            return db.evaCourseEnrollments.Count(e => e.evaCourseEnrollmentID == id) > 0;
        }
    }
}