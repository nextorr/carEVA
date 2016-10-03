using carEVA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace carEVA.Utils
{
    public static class enrollmentUtils
    {
        /// <summary>
        /// increment the score of the given enrollment by the given score. this does not persis hanges to the database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="EnrollmentID">Enrollment to count</param>
        /// <param name="score">amount to increment the stored score by.</param>
        /// <returns></returns>
        public static int incrementScore(carEVAContext context, int EnrollmentID, int score)
        {
            evaCourseEnrollment enrollment = context.evaCourseEnrollments.Find(EnrollmentID);
            if(enrollment == null)
            {
                return -1;
            }
            enrollment.currentScore = enrollment.currentScore + score;
            context.Entry(enrollment).State = EntityState.Modified;
            return 1;
        }
        public static int incrementCompletedLessons(carEVAContext context, int EnrollmentID)
        {
            evaCourseEnrollment enrollment = context.evaCourseEnrollments.Find(EnrollmentID);
            if(enrollment == null)
            {
                return -1;
            }
            enrollment.completedLessons++;
            context.Entry(enrollment).State = EntityState.Modified;
            return 1;
        }
        public static int updateScoreAndCompletedLessons(carEVAContext context, int EnrollmentID, int scoreDiff, int passedDiff)
        {
            evaCourseEnrollment enrollment = context.evaCourseEnrollments.Find(EnrollmentID);
            if (enrollment == null)
            {
                return -1;
            }
            enrollment.currentScore = enrollment.currentScore + scoreDiff;
            enrollment.completedLessons = enrollment.completedLessons + passedDiff;
            context.Entry(enrollment).State = EntityState.Modified;
            return 1;
        }
    }
}