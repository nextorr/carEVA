using carEVA.Models;
using carEVA.Utils;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace carEVA.Controllers.API
{
    public class evaExternalLogInController : ApiController
    {
        carEVAContext db = new carEVAContext();
        // GET: api/evaExternalLogIn
        public IEnumerable<string> Get()
        {
            return new string[] { "not implemented" };
        }

        // GET: api/evaExternalLogIn/5
        public string Get(int id)
        {
            return "not implemented";
        }

        // POST: api/evaExternalLogIn
        [ResponseType(typeof(evaLogIn))]
        public async Task<IHttpActionResult> Post([FromBody] evaLogIn evaLogIn)
        {
            if (!ModelState.IsValid)
            {
                evaLogUtils.logErrorMessage("invalid model",
                    this.ToString(), nameof(this.Post));
                return BadRequest(ModelState);
            }
            if (evaLogIn == null)
            {
                evaLogUtils.logErrorMessage("No information received",
                    this.ToString(), nameof(this.Post));
                return BadRequest("No information received");
            }
            if (!evaLogIn.containsValidInfo())
            {
                evaLogUtils.logErrorMessage("No user domain sent at log in",
                    this.ToString(), nameof(this.Post));
                return BadRequest("Bad formating, No user domain received");
            }

            evaSignInManager evaManager = new evaSignInManager(db
                , HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>());
            evaSignInResult authResult = await evaManager.signInExternalUser(evaLogIn, true);
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
                    evaLogIn.user, e, this.ToString(), nameof(this.Post));
                return BadRequest("ERROR : 400, cannot create eva user");
            }

            //here we are ignoring the route value
            evaLogIn.passKey = evaManager.publicKey;
            //TODO: see the interactions with this api and check if we can change this
            return CreatedAtRoute("DefaultApi", new { id = 1 }, evaLogIn);
        }

        // PUT: api/evaExternalLogIn/5
        public void Put(int id, [FromBody]string value)
        {
            
        }

        // DELETE: api/evaExternalLogIn/5
        public void Delete(int id)
        {
        }
    }
}
