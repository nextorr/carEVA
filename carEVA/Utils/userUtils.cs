using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            return context.evaUsers.Where(u => u.publicKey == publicKey).Single().evaUserID;
        }

        //---------------------------------------------------------------------------------------------

        public static int organizationIdFromKey(carEVAContext context, string publicKey)
        {
            return context.evaUsers.Where(u => u.publicKey == publicKey).Single().evaOrganizationID; 
        }

        //---------------------------------------------------------------------------------------------
        public static int incrementEnrolledCourses(carEVAContext context, string publicKey)
        {
            evaUser currentUser = context.evaUsers.Where(u => u.publicKey == publicKey).Single();
            if(currentUser == null)
            {
                return -1;
            }
            currentUser.totalEnrollments++;
            context.Entry(currentUser).State = EntityState.Modified;
            return 1;
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
                "update evausers set totalEnrollments ="+
                "(select COUNT(*) from evacourseenrollments "+
                "where evacourseenrollments.evauserID = evausers.evauserID)"
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
    }
}