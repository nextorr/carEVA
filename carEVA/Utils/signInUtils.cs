using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using carEVA.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace carEVA.Utils
{
    //
    // Summary:
    //     Possible results from a sign in attempt using all the eva providers
    public enum evaSignInResult
    {
        //
        // Summary:
        //     Sign in was successful
        Success = 0,
        //
        // Summary:
        //     User is locked out
        LockedOut = 1,
        //
        // Summary:
        //     Sign in requires addition verification (i.e. two factor)
        RequiresVerification = 2,
        //
        // Summary:
        //     Sign in failed
        Failure = 3,
        //
        // The user exists on ASPnet but not on evaBaseUsers
        // However tryin to create the user from the enterprise logIn provider failed
        // Most probably the password was incorrect
        FailedCreateFromInconsistency = 4,
        //
        // This is a system failure as everithing worked ok 
        // but updating the password on aspnet failed
        FailedToUpdateAspPassword = 5,
        //
        // the account has been disabled at the eva level
        // still all the data exists on the DB
        disabledAccount = 6
    }
    //*********************************************************************************************
    public class evaSignInManager
    {
        private carEVAContext db;
        private ApplicationSignInManager signInManager;
        UserManager<ApplicationUser> manager;
        private evaDataProtectionProvider provider;
        //currently selected user
        private evaBaseUser workingUser;
        public string publicKey { get; private set; }
        private sidcarUserServiceReference.WSIntegracionSoapClient sidcarProxy = new sidcarUserServiceReference.WSIntegracionSoapClient();
        //private class to handle the response and error from the SIDCAR service
        //---------------------------------------------------------------------------------------------
        private class userServiceResponse
        {
            public string statusMsg { get; set; }
            public evaBaseUser userInfo { get; set; }
        }
        //---------------------------------------------------------------------------------------------
        public evaSignInManager(carEVAContext context, ApplicationSignInManager _signInManager)
        {
            db = context;
            signInManager = _signInManager;
            manager = new UserManager<ApplicationUser>
            (new UserStore<ApplicationUser>(new ApplicationDbContext()));
            provider = new evaDataProtectionProvider();
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// loads into workingUsers the current user the class has reference to
        /// </summary>
        /// <param name="userName">the full username with domain of the user</param>
        private void loadUser(string userName) {
            workingUser = db.evaBaseUser.Where(m => m.userName == userName).SingleOrDefault();
        }
        //---------------------------------------------------------------------------------------------
        public async Task<evaSignInResult> signInOrganizationUser(evaLogIn _evaLogIn, bool updatePublicKey)
        {
            //load the working user
            loadUser(_evaLogIn.userAndDomain);

            if (!workingUser.isActive) {
                //return inmediately if the user is inactive.
                return evaSignInResult.disabledAccount;
            }
            //create a new or keep the existing public key
            if (updatePublicKey)
            {
                publicKey = Guid.NewGuid().ToString();
            }
            else
            {
                publicKey = workingUser.publicKey;
            }
            //plug in here the different login provider to integrate to authentication systems
            //depending on the organization
            switch (_evaLogIn.domain)
            {
                case "car.gov.co":
                    return await signInSIDCAR(_evaLogIn);
                default:
                    evaLogUtils.logWarningMessage("non existing domain logIn attempt " 
                        + _evaLogIn.userAndDomain + " Exist in ASP logins but not on evaLogIns"
                        ,this.ToString(), nameof(this.signInOrganizationUser));
                    return evaSignInResult.Failure;
            }
        }
        //---------------------------------------------------------------------------------------------
        public async Task<evaSignInResult> signInSIDCAR(evaLogIn _evaLogIn)
        {
            //attempt to log ing using the MVC identity system, this is expected to work once the user is created
            //and will validate login even if there is a problem with the SIDCAR service.
            //append the car.gov.co suffix to the username.
            var resultSIM = await signInManager.PasswordSignInAsync((_evaLogIn.userAndDomain), _evaLogIn.passKey, false, shouldLockout: false);
            userServiceResponse evaBaseUserFromAsp;
            //from the start check if the user is active on the eva model

            switch (resultSIM)
            {
                case SignInStatus.Success:
                    //its an existing user, so update the public key.
                    //bool updateResult = updateUserPublicKey(_evaLogIn.user, _evaLogIn.domain, publicKey);
                    bool updateResult = updateUserPublicKey();
                    if (!updateResult)
                    {
                        //this is model inconsistency: the user exist in aspNetUsers but not on evaUsers
                        //still, create the user on the eva model using the login data
                        evaBaseUserFromAsp = await buildEvaUserFromAsp(_evaLogIn.user, _evaLogIn.passKey);
                        if (evaBaseUserFromAsp.statusMsg == "ok")
                        {
                            //add the user to the eva model database
                            db.evaBaseUser.Add(evaBaseUserFromAsp.userInfo);
                            evaLogUtils.logWarningMessage("model inconsistency, user " + _evaLogIn.userAndDomain +
                                                            " Exist in ASP logins but not on evaLogIns",
                                                            this.ToString(), nameof(this.signInSIDCAR));
                        }
                        else
                        {
                            return evaSignInResult.FailedCreateFromInconsistency;
                        }
                    }
                    //TODO: this is not persisting changes on the database
                    break;
                case SignInStatus.LockedOut:
                    return evaSignInResult.LockedOut;
                case SignInStatus.RequiresVerification:
                    return evaSignInResult.RequiresVerification;
                case SignInStatus.Failure:
                    //the user or password used for log in is incorrect.
                    ApplicationUser aspUser = await manager.FindByNameAsync(_evaLogIn.userAndDomain);
                    if (aspUser != null)
                    {
                        //the user exists, so is an incorrect password on ASP
                        //here maybe the user updated the password on sidcar so try to sync the password
                        //first check if the user exist in the eva model
                        //check for all the user types, as in the model definition all types can login
                        int x = await db.evaBaseUser.Where(l => l.userName == (_evaLogIn.userAndDomain)).CountAsync();
                        if (x <= 0)
                        {
                            //this is model inconsistency: the user exist in aspNetUsers but not on evaUsers
                            //still, create the user on the eva model using the login data
                            evaBaseUserFromAsp = await buildEvaUserFromAsp(_evaLogIn.user, _evaLogIn.passKey);
                            if (evaBaseUserFromAsp.statusMsg == "ok")
                            {
                                //add the user to the eva model database
                                db.evaBaseUser.Add(evaBaseUserFromAsp.userInfo);
                                evaLogUtils.logWarningMessage("model inconsistency, user " + _evaLogIn.userAndDomain +
                                                                " Exist in ASP logins but not on evaLogIns",
                                                                this.ToString(), nameof(this.signInSIDCAR));
                            }
                            else
                            {
                                return evaSignInResult.FailedCreateFromInconsistency;
                            }
                        }
                        else
                        {
                            //the user exist on the eva model, so attemp to get info from EXTERNAL PROVIDER
                            evaBaseUserFromAsp = await getSidcarUser(_evaLogIn.user, _evaLogIn.passKey);
                            if (evaBaseUserFromAsp.statusMsg == "ok")
                            {
                                //the password IS out of sync, update the password on aspnetUser
                                string resetToken;
                                try
                                {
                                    manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("passwordReset"));
                                    resetToken = await manager.GeneratePasswordResetTokenAsync(aspUser.Id);
                                }
                                catch (Exception e)
                                {
                                    evaLogUtils.logErrorMessage("Could not update password from SIDCAR: " 
                                        + _evaLogIn.userAndDomain,
                                        this.ToString(), nameof(this.signInSIDCAR));
                                    return evaSignInResult.FailedToUpdateAspPassword;
                                }
                                IdentityResult pwResult = await manager.ResetPasswordAsync(aspUser.Id, resetToken, _evaLogIn.passKey);
                                if (!pwResult.Succeeded)
                                {
                                    evaLogUtils.logErrorMessage("Could not update password from SIDCAR: " + _evaLogIn.userAndDomain,
                                                this.ToString(), nameof(this.signInSIDCAR));
                                    return evaSignInResult.FailedToUpdateAspPassword;
                                }
                                else
                                {
                                    //if the result succeed, update the user public key
                                    //bool updatePK = updateUserPublicKey(_evaLogIn.user, _evaLogIn.domain, publicKey);
                                    bool updatePK = updateUserPublicKey();
                                }
                            }
                            else
                            {
                                //invalid credentials on SIDCAR as in ASP
                                return evaSignInResult.Failure;
                            }
                        }
                    }
                    else
                    {
                        //the user does not exist, create from SIDCAR
                        evaBaseUserFromAsp = await getSidcarUser(_evaLogIn.user, _evaLogIn.passKey);
                        if (evaBaseUserFromAsp.statusMsg == "ok")
                        {
                            //create the user.
                            ApplicationUser aspnetUser = new ApplicationUser
                            {
                                UserName = evaBaseUserFromAsp.userInfo.userName,
                                Email = evaBaseUserFromAsp.userInfo.email
                            };
                            var result = await manager.CreateAsync(aspnetUser, _evaLogIn.passKey);
                            if (result.Succeeded)
                            {
                                //build the relation with aspnetusers
                                evaBaseUserFromAsp.userInfo.aspnetUserID = aspnetUser.Id;
                                evaBaseUserFromAsp.userInfo.publicKey = publicKey;

                                db.evaBaseUser.Add(evaBaseUserFromAsp.userInfo);
                            }
                        }
                        else
                        {
                            //invalid credentials on SIDCAR ason ASP
                            return evaSignInResult.Failure;
                        }
                    }
                    break;
            }
            return evaSignInResult.Success;
        }
        //---------------------------------------------------------------------------------------------
        public async Task<evaSignInResult> signInExternalUser(evaLogIn _evaLogIn, bool updatePublicKey)
        {
            //load the working user
            loadUser(_evaLogIn.userAndDomain);
            //since this is an External user, the authentication is done exclusevely using 
            //the ASP net identity system
            publicKey = Guid.NewGuid().ToString();
            //create a new or keep the existing public key
            if (updatePublicKey)
            {
                publicKey = Guid.NewGuid().ToString();
            }
            else
            {
                publicKey = workingUser.publicKey;
            }
            var resultSIM = await signInManager.PasswordSignInAsync((_evaLogIn.userAndDomain), _evaLogIn.passKey, false, shouldLockout: false);
            switch (resultSIM)
            {
                case SignInStatus.Success:
                    //its an existing user, so update the public key.
                    //bool updateResult = updateUserPublicKey(_evaLogIn.user, _evaLogIn.domain, publicKey);
                    bool updateResult = updateUserPublicKey();
                    if (!updateResult)
                    {
                        //Cannot update the public key, the user exist on AspNetUsers but not on eva
                        //since the user must complete a form to sign In according to a pre - register
                        //we must return a error and delete this record from AspNet as is a ghost register.
                        userUtils.deleteAspNetIdentity(_evaLogIn.userAndDomain, manager);
                        return evaSignInResult.Failure;
                    }
                    break;
                case SignInStatus.LockedOut:
                    return evaSignInResult.LockedOut;
                case SignInStatus.RequiresVerification:
                    return evaSignInResult.RequiresVerification;
                case SignInStatus.Failure:
                    //here there is no alternate login system to fall back, so its a failure of login
                    return evaSignInResult.Failure;
                default:
                    break;
            }
            return new evaSignInResult();
        }
        //---------------------------------------------------------------------------------------------
        public async Task<string> createExternalLogin(string UserName, string email, string password)
        {
            //here we expect, sometimes the email field to be null
            if (String.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                //email has not beent previously set, by default use the username
                //since external users have a "area" domain its easy to identity who has got a default password
                email = UserName;

            }
            //creater the user in the aspnet login system
            ApplicationUser aspnetUser = new ApplicationUser
            {
                UserName = UserName,
                Email = email,
            };
            var result = await manager.CreateAsync(aspnetUser, password);
            if (result.Succeeded)
            {
                //build the relation with aspnetusers
                return aspnetUser.Id;
            }
            return null;
        }
        //---------------------------------------------------------------------------------------------
        //TODO: marked to delete.
        /// <summary>
        /// method to update the public key for a given user, note this does not persist changes in database
        /// </summary>
        /// <param name="userName">user to re generate the public key</param>
        private bool updateUserPublicKey(string userName, string domain, string publicKey)
        {
            //exclude instructors from this query as they cannot take courses
            //exclude inactive users
            //query base users because we want all user types except for instructors.
            workingUser = db.evaBaseUser.Where(l => l.userName == (userName+ "@" + domain)
                        && !(l is evaInstructor)).SingleOrDefault();
            if (workingUser != null)
            {
                workingUser.publicKey = publicKey;
                evaLogUtils.logInfoMessage("renewed Public key for user: " + userName,
                    this.ToString(), nameof(this.signInSIDCAR));
                //the operation succeded
                return true;
            }
            //the operation failed, no valid user found on the eva model
            return false;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// utilize all the internal variables, initializated at the start to update keys.
        /// </summary>
        /// <returns></returns>
        private bool updateUserPublicKey()
        {
            //if the working user is null the login provider decides what to do
            if (workingUser != null)
            {
                workingUser.publicKey = publicKey;
                evaLogUtils.logInfoMessage("renewed Public key for user: " + workingUser.userName,
                    this.ToString(), nameof(this.signInSIDCAR));
                //the operation succeded
                return true;
            }
            //the operation failed, no valid user found on the eva model
            return false;
        }

        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// Calls the SIDCAR service and return the information for the given username and pass. if it fails 
        /// logs the info and return a status message
        /// </summary>
        /// <param name="userName">user to query the service</param>
        /// <param name="Pass">password for the user</param>
        /// <returns>response.userinfo: evauser data response.statusdata: string with status information</returns>
        private async Task<userServiceResponse> getSidcarUser(string userName, string Pass)
        {
            userServiceResponse response = new userServiceResponse();
            sidcarUserServiceReference.SignInJsonResponse sidcarResponse = await sidcarProxy.SignInJsonAsync(userName, Pass, "EVA");
            //validate the response from the server
            //According to specification if the query is invalid the service will send an error
            if (sidcarResponse.Body.SignInJsonResult.StartsWith("ERROR"))
            {
                evaLogUtils.logErrorMessage("Invalid service Query, SIDCAR service responded with " + sidcarResponse.Body.SignInJsonResult,
                    this.ToString(), nameof(this.getSidcarUser));
                response.statusMsg = "invalid query";
                return response;
            }
            //here the response is OK to parse
            JObject jsonResult = JObject.Parse(sidcarResponse.Body.SignInJsonResult);
            //check if the response is parsed correctly
            if (jsonResult == null || !jsonResult.HasValues)
            {
                string error = "Parsing SIDCAR response failed";
                evaLogUtils.logErrorMessage(error + " SIDCAR service responded with " + sidcarResponse.Body.SignInJsonResult,
                    this.ToString(), nameof(this.getSidcarUser));
                response.statusMsg = error;
                return response;
                //return InternalServerError(new Exception("sidcar negotiation failed"));
            }
            string serviceMessage = (string)jsonResult["MsngRespuesta"];
            //According to specification, the service returns in MsngRespuesta its status on the login
            if (!serviceMessage.StartsWith("OK"))
            {
                string error = "Access denied, invalid user or password";
                evaLogUtils.logErrorMessage(error + " by user:  " + userName,
                    this.ToString(), nameof(this.getSidcarUser));
                response.statusMsg = error;
                return response;
            }

            //the parsed response is valid from here.

            evaUser newUser = new evaUser
            {
                userName = (string)(jsonResult["Login"]) + "@car.gov.co",
                fullName = (string)jsonResult["Nombre"],
                email = (string)jsonResult["EMail"],
                gender = (string)jsonResult["Sexo"],
                areaCode = (string)jsonResult["IDOficinaActual"],
                isActive = (bool)jsonResult["Activo"],
                totalEnrollments = 0,
                completedCatalogCourses = 0,
                completedRequiredCourses = 0,
                evaOrganizationID = 1,
                evaOrganizationAreaID = organizationUtils.getOrganizationAreaIdFromCode(db, (string)jsonResult["IDOficinaActual"]),
            };
            response.userInfo = newUser;
            response.statusMsg = "ok";
            return response;
        }
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        private async Task<userServiceResponse> buildEvaUserFromAsp(string userName, string Pass)
        {
            //first, call the sidcar service with the login information, as it contais the user data
            userServiceResponse response = await getSidcarUser(userName, Pass);
            if (response.statusMsg != "ok")
            {
                //invalid response, probably invalid password
                return response;
            }
            //a valid response, so now get asp user data and complement the EVA user,
            //specially the aspnetuserID softkey
            ApplicationUser aspnetUser = await manager.FindByNameAsync(response.userInfo.userName);
            response.userInfo.aspnetUserID = aspnetUser.Id;
            //assign a new public key to the user
            response.userInfo.publicKey = Guid.NewGuid().ToString();
            return response;
        }
        //---------------------------------------------------------------------------------------------

    }
}