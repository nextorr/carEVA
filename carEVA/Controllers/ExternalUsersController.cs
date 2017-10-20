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
using carEVA.Utils;
using Microsoft.AspNet.Identity;
using System.Reflection;
using carEVA.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace carEVA.Controllers
{
    [Authorize]
    public class ExternalUsersController : Controller
    {
        private carEVAContext db = new carEVAContext();

        // GET: ExternalUsers
        public async Task<ActionResult> Index(int? externalAreaID)
        {
            ViewBag.externalAreaID = new SelectList(db.evaOrganizationAreas
                .Where(m => m.isExternal == true), "evaOrganizationAreaID", "name");
            if (externalAreaID != null)
            {
                evaOrganizationArea area = await db.evaOrganizationAreas.FindAsync(externalAreaID);
                if (area.isExternal == false)
                {
                    return (new HttpStatusCodeResult(HttpStatusCode.BadRequest));
                }
                List<evaBaseUser> userList = area.usersInGroup.ToList();
                if (userList.Count == 0)
                {
                    //if the list is empty create a dummy element to inform the user of this
                    //and to send the areaID so a new one can be created
                    evaUser dummyUser = new evaUser()
                    {
                        fullName = "no hay usuarios registrados en este grupo",
                        evaOrganizationAreaID = (int)externalAreaID
                    };
                    userList.Add(dummyUser);
                }
                if (Request.IsAjaxRequest())
                {
                    // see https://stackoverflow.com/questions/3669760/iqueryable-oftypet-where-t-is-a-runtime-type
                    // using reflection to determine wich type of list view to display
                    Type myType = userList.FirstOrDefault().GetType();
                    MethodInfo method = typeof(Enumerable).GetMethod("OfType");
                    MethodInfo generic = method.MakeGenericMethod(myType);
                    var result = (IEnumerable<object>)generic
                        .Invoke(null, new object[] { userList });
                    return PartialView(userList.FirstOrDefault().getIndexViewName, result);
                }
                return View(userList);
            }
            return View(new List<evaBaseUser>());
        }

        // GET: ExternalUsers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaUser evaUser = await db.evaUsers.FindAsync(id);
            if (evaUser == null)
            {
                return HttpNotFound();
            }
            return View(evaUser);
        }

        // GET: ExternalUsers/Create
        public async Task<ActionResult> Create(int externalAreaID, string message)
        {
            evaOrganizationArea area = db.evaOrganizationAreas.Find(externalAreaID);
            ViewBag.areaName = area.name;
            ViewBag.Message = message; //used to show a success message to the user

            //use a dummy user to send default data to the view
            evaBaseUser dummyUser = externalUsers.createInstance(area.externalType);
            dummyUser.evaOrganizationID = await userUtils.organizationIdFromAspIdentity(db, User.Identity.GetUserId());
            dummyUser.evaOrganizationAreaID = externalAreaID;
            dummyUser.completedCatalogCourses = 0;
            dummyUser.completedRequiredCourses = 0;
            dummyUser.totalEnrollments = 0;
            dummyUser.aspnetUserID = "";
            dummyUser.publicKey = "";
            dummyUser.areaCode = area.areaCode;
            dummyUser.isActive = false;
            //the specific view for each especific external user is specified here
            return View(dummyUser.getCreateViewName, dummyUser);
        }

        // POST: ExternalUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,userName,email,fullName,aspnetUserID,areaCode,gender,publicKey,isActive,totalEnrollments,completedCatalogCourses,completedRequiredCourses,evaOrganizationAreaID,evaOrganizationID")] evaUser evaUser)
        {
            if (ModelState.IsValid)
            {
                db.evaUsers.Add(evaUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", new {
                    externalAreaID = evaUser.evaOrganizationAreaID,
                    message = "Usuario " + evaUser.fullName + " Pre- inscrito" });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        //implementing the create specifics for creating car defensores del agua.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvaCarDefensoresAgua([Bind(Include = "ID,userName,email,fullName,aspnetUserID,areaCode"
            +",gender,publicKey,isActive,totalEnrollments,completedCatalogCourses,completedRequiredCourses,evaOrganizationAreaID,evaOrganizationID" 
            + ", institucionEducativa, tipoDocumento, numeroDocumento, edad, municipio, gradoEstudio")] evaCarDefensoresAgua evaUser)
        {
            if (ModelState.IsValid)
            {
                evaUser.userName += "@defensores.car.gov.co";
                db.evaCarDefensoresAgua.Add(evaUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Create", new
                {
                    externalAreaID = evaUser.evaOrganizationAreaID,
                    message = "Usuario " + evaUser.fullName + " Pre- inscrito"
                });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: ExternalUsers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaBaseUser evaUser = await db.evaBaseUser.FindAsync(id);
            if (evaUser == null)
            {
                return HttpNotFound();
            }
            return View(evaUser.getEditViewName, evaUser);
        }

        // POST: ExternalUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,userName,email,fullName,aspnetUserID,areaCode,gender,publicKey,isActive,totalEnrollments,completedCatalogCourses,completedRequiredCourses,evaOrganizationAreaID,evaOrganizationID")] evaUser evaUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {externalAreaID = evaUser.evaOrganizationAreaID });
            }
            return View(evaUser);
        }

        //implementing the create specifics for creating car defensores del agua.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEvaCarDefensoresdelAgua([Bind(Include = "ID,userName,email,fullName,aspnetUserID,areaCode,gender,publicKey,isActive"
            +",totalEnrollments,completedCatalogCourses,completedRequiredCourses,evaOrganizationAreaID,evaOrganizationID"
            +", institucionEducativa, tipoDocumento, numeroDocumento, edad, municipio, gradoEstudio")] evaCarDefensoresAgua evaUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { externalAreaID = evaUser.evaOrganizationAreaID });
            }
            return View(evaUser);
        }

        // GET: ExternalUsers/userRegistration
        //this ask for name and ID to load the corresponding inscription form
        public ActionResult userRegistration(bool? documentError)
        {
            if (documentError == true)
            {
                ModelState.AddModelError("identificationNumber", "Documento no encontrado");
            }
            return View();
        }

        // POST: ExternalUsers/userRegistration
        //this ask for name and ID to load the corresponding inscription form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult userRegistration([Bind(Include = "userID, userFullName, documentType, identificationNumber")]externalUserViewModel externalID)
        {
            if (!ModelState.IsValid)
            {
                //the model bindig validation fails
                ModelState.AddModelError("userFullName", "Usuario no encontrado, Debes seleccionarlo de la lista");
                return View();
            }

            evaBaseUser storedUser = db.evaBaseUser.Find(externalID.userID);

            if (storedUser == null)
            {
                //TODO: if the info is correct serve the corresponding FORM
                //return an error otherwise
                return HttpNotFound();
                
            }

            //success, use the specific controller implementation
            return RedirectToAction(storedUser.registerUserControllerName()
                , new { userID = externalID.userID, idNumber = externalID.identificationNumber });
            
        }

        //the specific from definitions, depending on the type of user we want to register
        //register eva car niños defensores del agua
        public ActionResult registerEvaCarDefensoresdelAgua(int userID, long idNumber)
        {
            evaCarDefensoresAgua defensoresUser = db.evaCarDefensoresAgua.Find(userID);
            if (defensoresUser.numeroDocumento != idNumber)
            {
                //TODO, inform the user the ID is invalid
                return RedirectToAction("userRegistration", new { documentError = true });

            }
            return View(defensoresUser);
        }
        //implementing the create specifics for creating car defensores del agua.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> registerEvaCarDefensoresdelAgua([Bind(Include = "ID,userName,email,fullName,aspnetUserID,areaCode,gender,publicKey,isActive"
            +",totalEnrollments,completedCatalogCourses,completedRequiredCourses,evaOrganizationAreaID,evaOrganizationID"
            +", institucionEducativa, tipoDocumento, numeroDocumento, edad, municipio, gradoEstudio")] evaCarDefensoresAgua evaUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("passwordEvaCarDefensoresdelAgua", new { userID = evaUser.ID });
            }
            return View(evaUser);
        }
        //especific implementation to prompt the user to create a new password
        public ActionResult passwordEvaCarDefensoresdelAgua(int userID)
        {
            evaCarDefensoresAgua defensoresUser = db.evaCarDefensoresAgua.Find(userID);
            if (defensoresUser == null)
            {
                //Here the user must exist, so if the fin fail is a bad request
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            return View(new carDefensoresPasswordViewModel {password= "",userInfo= defensoresUser  });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> passwordEvaCarDefensoresdelAgua([Bind(Include = "password")]string pw
            , [Bind(Include = "ID", Prefix ="userInfo")]int userID)
        {
            evaCarDefensoresAgua defensoresUser = db.evaCarDefensoresAgua.Find(userID);
            //and create it on aspnetUsers
            evaSignInManager evaManager = new evaSignInManager(db
                , HttpContext.GetOwinContext().Get<ApplicationSignInManager>());
            string aspnetGUID = await evaManager.createExternalLogin(defensoresUser.userName
                , defensoresUser.email, pw);
            if (aspnetGUID == null)
            {
                //the creation failed, 
                //TODO: give the user a recovery point
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

            }
            //build the relation with aspnetusers and external user
            defensoresUser.isActive = true;
            //activate the user, 
            defensoresUser.aspnetUserID = aspnetGUID;
            defensoresUser.publicKey = new Guid().ToString();
            //TODO: send to the success confirmation window
            return View("successEvaCarDefensoresAgua", defensoresUser);
        }

        //TODO test method for the autocomplete, migrate this to an API service
        public ActionResult Autocomplete(string term)
       {
            List <userFullName> filteredItems  = db.evaBaseUser
                .Where(x => x.fullName.IndexOf(term) >= 0)
                .Where(x => (!(x is evaUser) && !(x is evaInstructor)))
                .Where(x => !x.isActive)
                .Select(x => new userFullName { evaUserID = x.ID, fullName = x.fullName }).ToList();
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        // GET: ExternalUsers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evaBaseUser evaUser = await db.evaBaseUser.FindAsync(id);
            if (evaUser == null)
            {
                return HttpNotFound();
            }
            return View(evaUser);
        }

        // POST: ExternalUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //TODO: deleting a user deletes all its related data, we should only disable it or something
            //since we are deleting the entire record, there is no need to cast to specific types.
            evaBaseUser evaUser = await db.evaBaseUser.FindAsync(id);
            db.evaBaseUser.Remove(evaUser);
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
