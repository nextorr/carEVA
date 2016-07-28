﻿using System;
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
    public class ChaptersController : ApiController
    {
        private carEVAContext db = new carEVAContext();

        // GET: api/Chapters
        public IQueryable<Chapter> GetChapters()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.Chapters;
        }

        // GET: api/Chapters/5
        [ResponseType(typeof(Chapter))]
        public IHttpActionResult GetChapter(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            //Chapter chapter = db.Chapters.Find(id);
            var returnData = db.Chapters.Where(b => b.CourseID == id).Include(b => b.lessons);
            if (returnData == null)
            {
                return NotFound();
            }

            return Ok(returnData);
        }

        // PUT: api/Chapters/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutChapter(int id, Chapter chapter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chapter.ChapterID)
            {
                return BadRequest();
            }

            db.Entry(chapter).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(id))
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

        // POST: api/Chapters
        [ResponseType(typeof(Chapter))]
        public IHttpActionResult PostChapter(Chapter chapter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Chapters.Add(chapter);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = chapter.ChapterID }, chapter);
        }

        // DELETE: api/Chapters/5
        [ResponseType(typeof(Chapter))]
        public IHttpActionResult DeleteChapter(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null)
            {
                return NotFound();
            }

            db.Chapters.Remove(chapter);
            db.SaveChanges();

            return Ok(chapter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChapterExists(int id)
        {
            return db.Chapters.Count(e => e.ChapterID == id) > 0;
        }
    }
}