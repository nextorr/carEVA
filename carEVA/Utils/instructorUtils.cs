using carEVA.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace carEVA.Utils
{
    public static class instructorUtils
    {
        public static async Task<int> instructorIdFromAsp(carEVAContext context, string aspUserID, UserManager<ApplicationUser> userManager)
        {
            evaInstructor currentInstructor;
            currentInstructor = await context.evaInstructor.Where(i => i.aspnetUserID == aspUserID).FirstOrDefaultAsync();
            if (currentInstructor != null)
            {
                return currentInstructor.ID;
            }
            //if the user is null, attempt to find the user by user name
            //if it succeds log the info, that is an inconsistency model
            var currentUser = await userManager.FindByIdAsync(aspUserID);
            currentInstructor = await context.evaInstructor.Where(i => i.userName == currentUser.UserName).FirstOrDefaultAsync();
            evaLogUtils.logWarningMessage("Instructor model inconsistency" + currentUser.UserName, "instructorIdFromAsp", "instructorIdFromAsp");
            //then update the model
            if (currentInstructor != null)
            {
                currentInstructor.aspnetUserID = aspUserID;
                context.Entry(currentInstructor).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            //there must be a user, as the controler validates only instructors can access this
            return currentInstructor.ID;
        }
    }
}