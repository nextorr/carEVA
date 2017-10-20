using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using carEVA.Models;
using carEVA.ViewModels;

using System.Data.Entity.SqlServer;
using System.Web.Http.Description;
using carEVA.Utils;

namespace carEVA.Controllers.API
{
    public class externalLogInController : ApiController
    {
        private carEVAContext db;
        public externalLogInController()
        {
            db = new carEVAContext();
            //use this parameter if you dont want all the children to be populated
            db.Configuration.ProxyCreationEnabled = false;
        }
        public externalLogInController(carEVAContext context)
        {
            db = context;
            //use this parameter if you dont want all the children to be populated
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/ExternalLogIn
        [ResponseType(typeof(List<userFullName>))]
        public IHttpActionResult Get(string externalUserName)
        {
            //this is a way to search if a pattern exists inside a column in the database
            //it only works on SQL server
            //does not search for misspels, it takes the search pattern in the exact order.
            //see more on https://stackoverflow.com/questions/1033007/like-operator-in-entity-framework
            List<userFullName> userList = db.evaBaseUser
                .Where(x => SqlFunctions.PatIndex("%" + externalUserName + "%", x.fullName) > 0)
                .Where(x => (!(x is evaUser) && !(x is evaInstructor)))
                .Select(x => new userFullName { evaUserID = x.ID, fullName = x.fullName }).ToList();
            return Ok(userList);
        }

        // GET: api/ExternalLogIn/5
        /// <summary>
        /// check if the document ID matches the one registered on the database.
        /// </summary>
        /// <param name="id">evaBaseUserID to search</param>
        /// <returns></returns>
        [ResponseType(typeof(string))]
        public IHttpActionResult Get(int id, int docId)
        {
            evaCarDefensoresAgua userInfo;
            try
            {
                userInfo = db.evaCarDefensoresAgua.Find(id);
                if (userInfo.numeroDocumento == docId)
                {

                }
            }
            catch (InvalidOperationException e)
            {
                return NotFound();
            }
            return Ok();
        }

        // POST: api/ExternalLogIn
        public IHttpActionResult Post([FromBody]evaCarDefensoresAgua logInData)
        {
            if (!ModelState.IsValid)
            {
                evaLogUtils.logErrorMessage("invalid model state",
                    this.ToString(), nameof(this.Post));
                return BadRequest(ModelState);
            }
            if (logInData == null)
            {
                evaLogUtils.logErrorMessage("No information received",
                    this.ToString(), nameof(this.Post));
                return BadRequest("No information received");
            }
            //since the user is pre - registered, update the information
            //update the aspnet login and
            //and activate the user


            return Ok();
        }

        // PUT: api/apiExternalLogIn/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiExternalLogIn/5
        public void Delete(int id)
        {
        }
    }
}
