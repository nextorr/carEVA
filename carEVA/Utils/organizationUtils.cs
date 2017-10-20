using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using carEVA.Models;
using System.Data.Entity;

namespace carEVA.Utils
{
    public static class organizationUtils
    {
        /// <summary>
        /// increment the total course counters according with the requiredcourse parameter
        /// </summary>
        /// <param name="context">db context</param>
        /// <param name="organizationID">organization ID to modify</param>
        /// <param name="requiredCourse">flag that increment the corresponding counter</param>
        /// <returns></returns>
        public static int incrementCourseCounter(carEVAContext context, int organizationID, bool requiredCourse)
        {
            evaOrganization organization = context.evaOrganizations.Find(organizationID);
            if (organization == null)
            {
                return -1;
            }
            if (requiredCourse)
            {
                organization.totalRequiredCourses++;
            }
            else
            {
                organization.totalCatalogCourses++;
            }
            context.Entry(organization).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// decrement the total course counters according with the requiredcourse parameter, throws an exception if the course is not associated with the organization
        /// </summary>
        /// <param name="context">db context</param>
        /// <param name="organizationID">organization ID to modify</param>
        /// <param name="requiredCourse">flag that increment the corresponding counter</param>
        /// <returns></returns>
        public static int decrementCourseCounter(carEVAContext context, int organizationID, int courseID)
        {
            evaOrganization organization = context.evaOrganizations.Find(organizationID);
            evaOrganizationCourse organizationCourse = context.evaOrganizationCourses.Where(o => o.evaOrganizationID == organizationID
                && o.courseID == courseID).Single();
            if (organization == null)
            {
                return -1;
            }
            if (organizationCourse.required)
            {
                organization.totalRequiredCourses--;
            }
            else
            {
                organization.totalCatalogCourses--;
            }
            context.Entry(organization).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// updates totalCatalogCourses and totalReuiredCourses for all Organizations on the database
        /// </summary>
        /// <param name="context">database context where user entities are located</param>
        /// <returns>total number of entities modified, -1 if something failed</returns>
        public static int syncTotalCoursesCountes(carEVAContext context)
        {
            int totalEntitites = 0;
            //use RAW SQL as EF is ineficient at updating multiple entities
            //this are tested directly in SSMS
            totalEntitites = context.Database.ExecuteSqlCommand(
                "update evaorganizations set " +
                "totalcatalogcourses = " +
                "(select COUNT(*) from evaorganizationcourses "+
                "where evaorganizationcourses.evaorganizationID = evaorganizations.evaorganizationID "+
                "AND evaorganizationcourses.required = 'false'),"+
                "totalrequiredcourses = "+
                "(select COUNT(*) from evaorganizationcourses "+
                "where evaorganizationcourses.evaorganizationID = evaorganizations.evaorganizationID "+
                "AND evaorganizationcourses.required = 'true')"
                );

            //update the total counters for every Organization.
            //var organizations = context.evaOrganizations.Include(m => m.organizationCourses);
            //foreach(evaOrganization organizationItem in organizations)
            //{
            //    try
            //    {
            //        organizationItem.totalCatalogCourses = organizationItem.organizationCourses.Where(q => q.required == false).Count();
            //        organizationItem.totalRequiredCourses = organizationItem.organizationCourses.Where(q => q.required == true).Count();
            //    }
            //    catch (Exception)
            //    {
            //        return -1;
            //    }
            //    totalEntitites++;
            //}

            return totalEntitites;
        }
        //---------------------------------------------------------------------------------------------
        public static int getOrganizationAreaIdFromCode(carEVAContext context, string _areaCode)
        {
            int areaID = 0;
            try
            {
                areaID = context.evaOrganizationAreas.Where(c => c.areaCode == _areaCode).FirstOrDefault().evaOrganizationAreaID;
            }
            catch (Exception)
            {
                return context.evaOrganizationAreas.Where(c => c.areaCode == "1").FirstOrDefault().evaOrganizationAreaID;
            }
            
            return areaID;
        }
    }
}