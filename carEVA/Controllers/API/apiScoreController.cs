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
using carEVA.ViewModels;
using carEVA.Utils;
using System.Web.Mvc;

namespace carEVA.Controllers.API
{
    public class scoreController : ApiController
    {
        private carEVAContext db = new carEVAContext();
        // GET: api/score
        [ResponseType(typeof(userOverviewScore))]
        public IHttpActionResult Getscore(string publicKey)
        {
            evaUser currentUser;
            evaOrganization currentOrganization;
            userOverviewScore response;
            try
            {
                currentUser = db.evaUsers.Where(p => p.publicKey == publicKey).Include(p => p.CourseEnrollments).Single();
                currentOrganization = db.evaOrganizations.Where(p => p.evaOrganizationID == currentUser.evaOrganizationID).
                    Include(p => p.organizationCourses).Single();
            }
            catch (InvalidOperationException)
            {
                //report the service client that the key they are using is invalid.
                //view log notes for more info on how to return error messages.
                return BadRequest("100:ERROR the public key is invalid");
            }

            response = new userOverviewScore()
            {
                totalActiveEnrollments = currentUser.CourseEnrollments.Count(),
                totalCatalogCourses = currentOrganization.organizationCourses.Where(q => q.required == false).Count(),
                //TODO: complete this information when the evaluation logic in completed
                completedCatalogCourses = 0,
                totalRequiredCourses = currentOrganization.organizationCourses.Where(q => q.required == true).Count(),
                //TODO: complete this information when the evaluation logic in completed
                completedRequiredCourses = 0,
                
            };

            return Ok(response);
        }

        // GET: api/score/5
        public string Getscore(int id)
        {
            return "value";
        }

        // POST: api/score
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/score/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/score/5
        public void Delete(int id)
        {
        }
    }
}
