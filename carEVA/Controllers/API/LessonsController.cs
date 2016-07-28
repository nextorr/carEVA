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

namespace carEVA.Controllers.API
{
    public class LessonsController : ApiController
    {
        private carEVAContext db = new carEVAContext();

        // GET: api/Lessons
        public IQueryable<Lesson> GetLessons()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Lessons;
        }

        // GET: api/Lessons/5
        [ResponseType(typeof(Lesson))]
        public IHttpActionResult GetLesson(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        // PUT: api/Lessons/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLesson(int id, Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lesson.LessonID)
            {
                return BadRequest();
            }

            db.Entry(lesson).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
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

        // POST: api/Lessons
        [ResponseType(typeof(Lesson))]
        public IHttpActionResult PostLesson(Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Lessons.Add(lesson);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = lesson.LessonID }, lesson);
        }

        // DELETE: api/Lessons/5
        [ResponseType(typeof(Lesson))]
        public IHttpActionResult DeleteLesson(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return NotFound();
            }

            db.Lessons.Remove(lesson);
            db.SaveChanges();

            return Ok(lesson);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LessonExists(int id)
        {
            return db.Lessons.Count(e => e.LessonID == id) > 0;
        }
    }
}