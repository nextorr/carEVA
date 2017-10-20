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
    public class OrganizationAreasController : Controller
    {
        private carEVAContext db;
        public OrganizationAreasController()
        {
            db = new carEVAContext();
        }

        public OrganizationAreasController(carEVAContext context)
        {
            db = context;
        }

        // GET: OrganizationAreas
        public async Task<ActionResult> Index()
        {
            var evaOrganizationAreas = db.evaOrganizationAreas.Include(e => e.organization)
                .Where(e => e.isExternal == false);
            return View(await evaOrganizationAreas.ToListAsync());
        }

        // GET: OrganizationAreas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaOrganizationArea evaOrganizationArea = await db.evaOrganizationAreas.FindAsync(id);
            if (evaOrganizationArea == null)
            {
                return HttpNotFound();
            }
            return View(evaOrganizationArea);
        }

        // GET: OrganizationAreas/Create
        public ActionResult Create()
        {
            ViewBag.evaOrganizationID = new SelectList(db.evaOrganizations, "evaOrganizationID", "name");
            return View();
        }

        // POST: OrganizationAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "evaOrganizationAreaID,areaCode,name,nameAbreviation,isEnabled,isExternal,evaOrganizationID")] evaOrganizationArea evaOrganizationArea)
        {
            if (ModelState.IsValid)
            {
                db.evaOrganizationAreas.Add(evaOrganizationArea);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.evaOrganizationID = new SelectList(db.evaOrganizations, "evaOrganizationID", "name", evaOrganizationArea.evaOrganizationID);
            return View(evaOrganizationArea);
        }

        // GET: OrganizationAreas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaOrganizationArea evaOrganizationArea = await db.evaOrganizationAreas.FindAsync(id);
            if (evaOrganizationArea == null)
            {
                return HttpNotFound();
            }
            ViewBag.evaOrganizationID = new SelectList(db.evaOrganizations, "evaOrganizationID", "name", evaOrganizationArea.evaOrganizationID);
            return View(evaOrganizationArea);
        }

        // POST: OrganizationAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "evaOrganizationAreaID,areaCode,name,nameAbreviation,isEnabled,isExternal,evaOrganizationID")] evaOrganizationArea evaOrganizationArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaOrganizationArea).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.evaOrganizationID = new SelectList(db.evaOrganizations, "evaOrganizationID", "name", evaOrganizationArea.evaOrganizationID);
            return View(evaOrganizationArea);
        }

        // GET: OrganizationAreas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaOrganizationArea evaOrganizationArea = await db.evaOrganizationAreas.FindAsync(id);
            if (evaOrganizationArea == null)
            {
                return HttpNotFound();
            }
            return View(evaOrganizationArea);
        }

        // POST: OrganizationAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            evaOrganizationArea evaOrganizationArea = await db.evaOrganizationAreas.FindAsync(id);
            db.evaOrganizationAreas.Remove(evaOrganizationArea);
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
