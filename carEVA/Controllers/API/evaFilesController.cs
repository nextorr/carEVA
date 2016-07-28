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
    public class evaFilesController : ApiController
    {
        private carEVAContext db = new carEVAContext();

        // GET: api/evaFiles
        public IQueryable<evaFile> GetFiles()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Files;
        }

        // GET: api/evaFiles/5
        [ResponseType(typeof(evaFile))]
        public IHttpActionResult GetevaFile(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //we expect to receive the id of the lesson, from there get the chapter and course ID
            //TODO: here we are checking only for global files, 
            //we need a function to get the chapter files and the lesson files only
            var fullLesson = db.Lessons.Where(l => l.LessonID == id)
                .Include(b => b.Chapter).FirstOrDefault();
            var evaFiles = db.Files.Where(f => f.courseID == fullLesson.Chapter.CourseID);
            //evaFile evaFile = db.Files.Find(id);
            //if (evaFile == null)
            //{
            //    return NotFound();
            //}
            //return Ok(evaFile);
            if (evaFiles == null)
            {
                return NotFound();
            }
            return Ok(evaFiles);
            
        }

        // PUT: api/evaFiles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutevaFile(int id, evaFile evaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaFile.evaFileID)
            {
                return BadRequest();
            }

            db.Entry(evaFile).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!evaFileExists(id))
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

        // POST: api/evaFiles
        [ResponseType(typeof(evaFile))]
        public IHttpActionResult PostevaFile(evaFile evaFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Files.Add(evaFile);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = evaFile.evaFileID }, evaFile);
        }

        // DELETE: api/evaFiles/5
        [ResponseType(typeof(evaFile))]
        public IHttpActionResult DeleteevaFile(int id)
        {
            evaFile evaFile = db.Files.Find(id);
            if (evaFile == null)
            {
                return NotFound();
            }

            db.Files.Remove(evaFile);
            db.SaveChanges();

            return Ok(evaFile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool evaFileExists(int id)
        {
            return db.Files.Count(e => e.evaFileID == id) > 0;
        }
    }
}