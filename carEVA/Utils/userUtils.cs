﻿using carEVA.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace carEVA.Utils
{
    public static class userUtils
    {
        /// <summary>
        /// get the user ID for the given key, throws exception if multiple or no user found.
        /// </summary>
        /// <param name="context">db context</param>
        /// <param name="publicKey">key</param>
        /// <returns></returns>
        public static int userIdFromKey(carEVAContext context, string publicKey)
        {
            //TODO: check base user change stability
            //return context.evaUsers.Where(u => u.publicKey == publicKey).Single().ID;
            return context.evaBaseUser.Where(u => u.publicKey == publicKey).Single().ID;
        }

        //---------------------------------------------------------------------------------------------
        public static string publicKeyFromUserId(carEVAContext context, string _aspNetUserID)
        {
            return context.evaBaseUser.Where(u => u.aspnetUserID == _aspNetUserID).Single().publicKey;
        }
        //---------------------------------------------------------------------------------------------

        public static int organizationIdFromKey(carEVAContext context, string publicKey)
        {
            //TODO: check base user change stability
            //return context.evaUsers.Where(u => u.publicKey == publicKey).Single().evaOrganizationID; 
            return context.evaBaseUser.Where(u => u.publicKey == publicKey).Single().evaOrganizationID;
        }

        //---------------------------------------------------------------------------------------------
        public static async Task<int> organizationIdFromAspIdentity(carEVAContext context, string aspUserID)
        {
            //TODO: check base user change stability
            //As dic 2017 the evaModel only allows one base user type per ASP.NET key
            evaBaseUser currentUser = await context.evaBaseUser.Where(u => u.aspnetUserID == aspUserID).FirstOrDefaultAsync();
            return currentUser.evaOrganizationID;
        }
        //---------------------------------------------------------------------------------------------
        public static evaOrganizationArea areaFromAspIdentity(carEVAContext context, string aspUserID)
        {
            //TODO: check base user change stability
            evaBaseUser currentUser = context.evaBaseUser.Where(u => u.aspnetUserID == aspUserID).Single();
            return currentUser.organizationArea;
        }
        //---------------------------------------------------------------------------------------------
        public static int incrementEnrolledCourses(carEVAContext context, string publicKey)
        {
            //TODO: check base user change stability
            evaBaseUser currentUser = context.evaBaseUser.Where(u => u.publicKey == publicKey).Single();
            if(currentUser == null)
            {
                return -1;
            }
            currentUser.totalEnrollments++;
            context.Entry(currentUser).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        public static bool decrementEnrolledCourses(carEVAContext context, int evaUserID)
        {
            evaBaseUser currenUser = context.evaBaseUser.Where(u => u.ID == evaUserID).Single();
            if (currenUser == null)
            {
                return false;
            }
            currenUser.totalEnrollments--;
            context.Entry(currenUser).State = EntityState.Modified;
            return true;
        }

        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// updates totalEnrollments, completedCatalogCourses and completedRequiredCourses for all users on the database
        /// </summary>
        /// <param name="context">database context where user entities are located</param>
        /// <returns>total number of entities modified, -1 if something failed</returns>
        public static int syncCoursesCountes(carEVAContext context)
        {
            int totalEntitites = 0;
            //use RAW SQL as EF is ineficient at updating multiple entities
            //this are tested directly in SSMS
            totalEntitites = context.Database.ExecuteSqlCommand(
                "update evabaseusers set totalEnrollments ="+
                "(select COUNT(*) from evacourseenrollments "+
                "where evacourseenrollments.evauserID = evabaseusers.ID"+
                " and evabaseusers.discriminator='evaUser')"
                );
            
            return totalEntitites;
        }
        //---------------------------------------------------------------------------------------------
        public static int syncAllCounters(carEVAContext context)
        {
            int result = 0;
            result = context.Database.ExecuteSqlCommand("exec sp_updateCounters");
            return result;
        }
        //---------------------------------------------------------------------------------------------
        public static string getValidPublicKey(carEVAContext context)
        {
            //this returns the nromerob user
            return context.evaUsers.FirstOrDefault().publicKey;
        }
        //---------------------------------------------------------------------------------------------
        public static bool deleteUserAndAspnetIdentity(int evaUserID, carEVAContext context, UserManager<ApplicationUser> userManager)
        {
            //TODO: check base user change stability
            evaUser currentUser = context.evaUsers.Find(evaUserID);
            if (currentUser == null)
            {
                return false;
            }
            context.evaUsers.Remove(currentUser);
            //TODO: we need to do some more validation here
            var aspnetUser = userManager.FindByName(currentUser.userName);
            var logins = aspnetUser.Logins;
            var userRoles = userManager.GetRoles(aspnetUser.Id);
            //remove the user logins
            foreach (var login in logins)
            {
                userManager.RemoveLogin(aspnetUser.Id, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }
            //remove th user roles if there is any
            foreach (var item in userRoles.ToList())
            {
                userManager.RemoveFromRole(aspnetUser.Id, item);
            }
            //finally remove the aspnetUser from the identity model
            userManager.Delete(aspnetUser);
            return true;
        }
        //---------------------------------------------------------------------------------------------
        public static void deleteAspNetIdentity(string userName, UserManager<ApplicationUser> userManager)
        {
            //TODO: we need to do some more validation here
            var aspnetUser = userManager.FindByName(userName);
            var logins = aspnetUser.Logins;
            var userRoles = userManager.GetRoles(aspnetUser.Id);
            //remove the user logins
            foreach (var login in logins)
            {
                userManager.RemoveLogin(aspnetUser.Id, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }
            //remove th user roles if there is any
            foreach (var item in userRoles.ToList())
            {
                userManager.RemoveFromRole(aspnetUser.Id, item);
            }
            //finally remove the aspnetUser from the identity model
            userManager.Delete(aspnetUser);
        }
    }
}