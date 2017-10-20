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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using carEVA.Utils;

namespace carEVA.Controllers
{
    [Authorize]
    public class ExternalAreasController : Controller
    {
        private carEVAContext db = new carEVAContext();
        private ApplicationDbContext appContext;
        private UserManager<ApplicationUser> userManager;

        //initialization constructor
        public ExternalAreasController()
        {
            appContext = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appContext));
        }

        // GET: ExternalAreas
        public async Task<ActionResult> Index()
        {
            var evaOrganizationAreas = db.evaOrganizationAreas.Include(e => e.organization)
                .Where(e => e.isExternal == true);
            return View(await evaOrganizationAreas.ToListAsync());
        }

        // GET: ExternalAreas/Details/5
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

        // GET: ExternalAreas/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.evaOrganizationID = new SelectList(db.evaOrganizations, "evaOrganizationID", "name");
            //use this temporal model to set default values for external courses on the view
            evaOrganizationArea areas = new evaOrganizationArea();
            areas.isExternal = true;
            areas.evaOrganizationID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            areas.isEnabled = true;
            return View(areas);
        }

        // POST: ExternalAreas/Create
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
            //use this temporal model to set default values for external courses on the view
            evaOrganizationArea areas = new evaOrganizationArea();
            areas.isExternal = true;
            areas.evaOrganizationID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            areas.isEnabled = true;
            return View(evaOrganizationArea);
        }

        // GET: ExternalAreas/Edit/5
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
            evaOrganizationArea.isEnabled = true;
            evaOrganizationArea.isExternal = true;
            return View(evaOrganizationArea);
        }

        // POST: ExternalAreas/Edit/5
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
            evaOrganizationArea.isEnabled = true;
            evaOrganizationArea.isExternal = true;
            return View(evaOrganizationArea);
        }

        // GET: ExternalAreas/Delete/5
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

        // POST: ExternalAreas/Delete/5
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
