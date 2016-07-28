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
using System.Threading.Tasks;
using carEVA.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace carEVA.Controllers.API
{
    public class apiEvaLogInsController : ApiController
    {
        private carEVAContext db = new carEVAContext();
        private sidcarUserServiceReference.WSIntegracionSoapClient sidcarProxy = new sidcarUserServiceReference.WSIntegracionSoapClient();
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: api/apiEvaLogIns
        public IQueryable<evaLogIn> GetevaLogIns()
        {
            return db.evaLogIns;
        }

        // GET: api/apiEvaLogIns/5
        [ResponseType(typeof(evaLogIn))]
        public IHttpActionResult GetevaLogIn(int id)
        {
            evaLogIn evaLogIn = db.evaLogIns.Find(id);
            if (evaLogIn == null)
            {
                return NotFound();
            }

            return Ok(evaLogIn);
        }

        // PUT: api/apiEvaLogIns/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutevaLogIn(int id, evaLogIn evaLogIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaLogIn.evaLogInID)
            {
                return BadRequest();
            }

            db.Entry(evaLogIn).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!evaLogInExists(id))
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

        // POST: api/apiEvaLogIns
        [ResponseType(typeof(evaLogIn))]
        public async Task<IHttpActionResult> PostevaLogIn([FromBody] evaLogIn evaLogIn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (evaLogIn == null) {
                return BadRequest("no information received");
            }

            sidcarUserServiceReference.SignInJsonResponse response = await sidcarProxy.SignInJsonAsync(evaLogIn.user, evaLogIn.pass, "EVA");
            JObject jsonResult = JObject.Parse(response.Body.SignInJsonResult);

            evaUser newUser = new evaUser
            {
                userName = (string)(jsonResult["Login"])+"@car.gov.co",
                fullName = (string)jsonResult["Nombre"],
                email = (string)jsonResult["EMail"],
                gender = (string)jsonResult["Sexo"],
                areaCar = (string)jsonResult["IDOficinaActual"],
            };

            ApplicationUser aspnetUser = new ApplicationUser { UserName = newUser.email, Email = newUser.email, userKey = Guid.NewGuid().ToString() };
            var result = await manager.CreateAsync(aspnetUser, evaLogIn.pass);
            newUser.aspnetUserID = aspnetUser.Id;

            //ApplicationUser aspnetUser = manager.Users.First();
            //var result = await manager.DeleteAsync(aspnetUser);



            evaLogIn.pass = aspnetUser.userKey;
            db.evaLogIns.Add(evaLogIn);
            db.SaveChanges();


            return CreatedAtRoute("DefaultApi", new { id = evaLogIn.evaLogInID }, evaLogIn);
        }

        // DELETE: api/apiEvaLogIns/5
        [ResponseType(typeof(evaLogIn))]
        public IHttpActionResult DeleteevaLogIn(int id)
        {
            evaLogIn evaLogIn = db.evaLogIns.Find(id);
            if (evaLogIn == null)
            {
                return NotFound();
            }

            db.evaLogIns.Remove(evaLogIn);
            db.SaveChanges();

            return Ok(evaLogIn);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool evaLogInExists(int id)
        {
            return db.evaLogIns.Count(e => e.evaLogInID == id) > 0;
        }
    }
}