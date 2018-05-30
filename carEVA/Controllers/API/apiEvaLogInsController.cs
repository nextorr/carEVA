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
using Microsoft.AspNet.Identity.Owin;
using carEVA.Utils;
using carEVA.ViewModels;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security.DataProtection;

namespace carEVA.Controllers.API
{
    public class evaLogInsController : ApiController
    {
        private carEVAContext db;
        public evaLogInsController()
        {
            db = new carEVAContext();
        }

        public evaLogInsController(carEVAContext context)
        {
            db = context;
        }
        // GET: api/evalogIns
        public IHttpActionResult GetevaLogIns()
        {
            //use this to test the log database
            db.Configuration.ProxyCreationEnabled = false;
            //return Ok(db.evaBaseUser.ToList()); //use this to see how the JSON serializer handle inheritance
            return Ok(new evaResponses("method not implemented", "Ok"));
        }

        // GET: api/evalogIns/5
        [ResponseType(typeof(evaLogIn))]
        public IHttpActionResult GetevaLogIn(int id)
        {
            return NotFound();
        }

        // PUT: api/evalogIns/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutevaLogIn(int id, evaLogIn evaLogIn)
        {
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/evalogIns
        [ResponseType(typeof(evaLogIn))]
        public async Task<IHttpActionResult> PostevaLogIn([FromBody] evaLogIn evaLogIn)
        {
            if (!ModelState.IsValid)
            {
                evaLogUtils.logErrorMessage("invalid model",
                    this.ToString(), nameof(this.PostevaLogIn));
                return BadRequest(ModelState);
            }
            if (evaLogIn == null) {
                evaLogUtils.logErrorMessage("No information received", 
                    this.ToString(), nameof(this.PostevaLogIn));
                return BadRequest("No information received");
            }
            if (!evaLogIn.containsValidInfo())
            {
                evaLogUtils.logErrorMessage("No user domain sent at log in",
                    this.ToString(), nameof(this.PostevaLogIn));
                return BadRequest("Bad formating, No user domain received");
            }

            evaSignInManager evaManager = new evaSignInManager(db
                , HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>());
            evaSignInResult authResult = await evaManager.signInOrganizationUser(evaLogIn, true);
            switch (authResult)
            {
                case evaSignInResult.Success:
                    //break inmediatly so it does not check for other conditions
                    break;
                case evaSignInResult.LockedOut:
                    return BadRequest("the Account is locked, try again later");
                case evaSignInResult.RequiresVerification:
                    return BadRequest("the Account is not verifyed");
                case evaSignInResult.Failure:
                    return BadRequest("Invalid user or Password");
                case evaSignInResult.FailedCreateFromInconsistency:
                    return BadRequest("Sistem error trying to authenticate");
                case evaSignInResult.FailedToUpdateAspPassword:
                    return BadRequest("Sistem error trying to authenticate");
                case evaSignInResult.disabledAccount:
                    return BadRequest("the Account is disabled");
                default:
                    return BadRequest("General system error");
            }

            try
            {
                //creates a new user or updates its public key
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //report the service client that the key they are using is invalid.
                evaLogUtils.logErrorMessage("cannot create eva user ",
                    evaLogIn.userAndDomain, e, this.ToString(), nameof(this.GetevaLogIn));
                return BadRequest("ERROR : 400, cannot create eva user");
            }

            //here we are ignoring the route value
            evaLogIn.passKey = evaManager.publicKey;
            //TODO: see the interactions with this api and check if we can change this
            return CreatedAtRoute("DefaultApi", new { id = 1 }, evaLogIn);
        }
        // DELETE: api/evalogIns/5
        [ResponseType(typeof(evaLogIn))]
        public IHttpActionResult DeleteevaLogIn(int id)
        {
            return NotFound();
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