using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carEVA.Models;

namespace carEVA.Controllers
{
    public class evaLogsController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: evaLogs
        public async Task<ActionResult> Index()
        {
            return View(await db.evaLogs.OrderByDescending(b => b.date).Take(50).ToListAsync());
        }

        // GET: evaLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaLog evaLog = await db.evaLogs.FindAsync(id);
            if (evaLog == null)
            {
                return HttpNotFound();
            }
            return View(evaLog);
        }

        // GET: evaLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: evaLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "evaLogID,level,caller,message,date")] evaLog evaLog)
        {
            if (ModelState.IsValid)
            {
                db.evaLogs.Add(evaLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(evaLog);
        }

        // GET: evaLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaLog evaLog = await db.evaLogs.FindAsync(id);
            if (evaLog == null)
            {
                return HttpNotFound();
            }
            return View(evaLog);
        }

        // POST: evaLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "evaLogID,level,caller,message,date")] evaLog evaLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(evaLog);
        }

        // GET: evaLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaLog evaLog = await db.evaLogs.FindAsync(id);
            if (evaLog == null)
            {
                return HttpNotFound();
            }
            return View(evaLog);
        }

        // POST: evaLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            evaLog evaLog = await db.evaLogs.FindAsync(id);
            db.evaLogs.Remove(evaLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
