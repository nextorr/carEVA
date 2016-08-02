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


            sidcarUserServiceReference.SignInJsonResponse response = await sidcarProxy.SignInJsonAsync(evaLogIn.user, evaLogIn.passKey, "EVA");
            //validate the response from the server
            //According to specification if the query is invalid the service will send an error
            if (response.Body.SignInJsonResult.StartsWith("ERROR"))
            {
                return BadRequest("invalid query");
            }
            //here the response is OK to parse
            JObject jsonResult = JObject.Parse(response.Body.SignInJsonResult);
            //check if the response is parsed correctly
            if (jsonResult == null || !jsonResult.HasValues)
            {
                return InternalServerError(new Exception("sidcar negotiation failed"));
            }
            string serviceMessage = (string)jsonResult["MsngRespuesta"];
            //According to specification, the service returns in MsngRespuesta its status on the login
            if (!serviceMessage.StartsWith("OK"))
            {
                return BadRequest("Access denied, invalid user or password");
            }
            //the parsed response is valid from here.

            evaUser newUser = new evaUser
            {
                userName = (string)(jsonResult["Login"]) + "@car.gov.co",
                fullName = (string)jsonResult["Nombre"],
                email = (string)jsonResult["EMail"],
                gender = (string)jsonResult["Sexo"],
                areaCar = (string)jsonResult["IDOficinaActual"],
                isActive = (bool)jsonResult["Activo"]

            };

            ApplicationUser aspnetUser = new ApplicationUser { UserName = newUser.userName, Email = newUser.email };
            var result = await manager.CreateAsync(aspnetUser, evaLogIn.passKey);
            if (result.Succeeded)
            {
                //its a new user
                newUser.aspnetUserID = aspnetUser.Id;
                //assign a new public key to the user
                newUser.publicKey = Guid.NewGuid().ToString();
                //adapt the eva log in information to return the public key
                evaLogIn.passKey = newUser.publicKey;
                evaLogIn.user = newUser.userName;
                //dont stonre anything on the eva log in table, we use it just as a functional view, 
                //but on the data side its just reduntant
                //db.evaLogIns.Add(evaLogIn);
                db.evaUsers.Add(newUser);
                db.SaveChanges();

                //return CreatedAtRoute("DefaultApi", new { id = evaLogIn.evaLogInID }, evaLogIn);
            }
            else
            {
                //its an existing user, update its public key
                string newPublicKey = Guid.NewGuid().ToString();
                var originalUser = db.evaUsers.Where(l => l.userName == newUser.userName).FirstOrDefault();
                if (originalUser != null)
                {
                    originalUser.publicKey = newPublicKey;
                    evaLogIn.passKey = originalUser.publicKey;
                    evaLogIn.user = originalUser.userName;

                }
                else
                {
                    //this must not happend, as its an inconsistency, 
                    //the user exist in asp logins but not in the user eva model
                    //but still, if this happens add the user log in
                    //TODO: log this information.
                    evaLogIn.passKey = newPublicKey;
                    evaLogIn.user = newUser.userName;
                    //db.evaLogIns.Add(evaLogIn);
                    newUser.publicKey = newPublicKey;
                    newUser.aspnetUserID = aspnetUser.Id;
                    db.evaUsers.Add(newUser);

                }
                db.SaveChanges();
            }

            //this piece is used to delete the first record on the aspnetuser table
            //ApplicationUser aspnetUser = manager.Users.First();
            //var result = await manager.DeleteAsync(aspnetUser);

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
        private bool evaLogInExists(string login)
        {
            return db.evaLogIns.Count(e => e.user == login) > 0;
        }
    }
}