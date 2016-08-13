using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using carEVA.Models;

namespace carEVA.Utils
{
    //*********************************************************************************************
    
    public static class courseUtils
    {
        /// <summary>
        /// adds one to the total quizes counter on the courses Entity
        /// returns -1 if the specifued course is not found
        /// does not persis data on the database, the calling method must save change to persis it
        /// </summary>
        /// <param name="context"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public static int incrementTotalQuizes(carEVAContext context, int courseID)
        {
            Course proxyCourse = context.Courses.Find(courseID);
            if(proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalQuizes++;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }

        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// decrease one to the total quizes counter on the courses Entity
        /// returns -1 if the specifued course is not found
        /// does not persis data on the database, the calling method must save change to persis it
        /// </summary>
        public static int decrementTotalQuizes(carEVAContext context, int courseID)
        {
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalQuizes--;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        public static int incrementTotalQuizesAndPoints(carEVAContext context, int courseID, int quizPoints)
        {
            /// <summary>
            /// adds one to the total quizes counter on the courses Entity
            /// returns -1 if the specifued course is not found
            /// does not persis data on the database, the calling method must save change to persis it
            /// </summary>
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalQuizes++;
            proxyCourse.totalPoints = proxyCourse.totalPoints + quizPoints;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// decrease one to the total quizes counter on the courses Entity
        /// returns -1 if the specifued course is not found
        /// does not persis data on the database, the calling method must save change to persis it
        /// </summary>
        public static int decrementTotalQuizesAndPoints(carEVAContext context, int courseID, int quizPoints)
        {
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalQuizes--;
            proxyCourse.totalPoints = proxyCourse.totalPoints - quizPoints;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// modifies the total points tracker for the given course using the difference between the new and previous values of quiz points
        /// </summary>
        /// <param name="context">database context</param>
        /// <param name="courseID">course entity to modify</param>
        /// <param name="newQuizPoints">modified value of the quiz points</param>
        /// <param name="previousQuizPoints">previous points for the given quiz</param>
        /// <returns></returns>
        public static int modifyTotalPoints(carEVAContext context, int courseID, int newQuizPoints, int previousQuizPoints)
        {
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            int pointDifference = previousQuizPoints - newQuizPoints;
            proxyCourse.totalPoints = proxyCourse.totalPoints + pointDifference;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// increase one to the total Lessons counter on the courses Entity
        /// returns -1 if the specifued course is not found
        /// does not persis data on the database, the calling method must save change to persis it
        /// </summary>
        public static int incrementTotalLessons(carEVAContext context, int courseID)
        {
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalLessons++;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }

        //---------------------------------------------------------------------------------------------
        /// <summary>
        /// decrement one to the total Lessons counter on the courses Entity
        /// and also reduce the number of questions associated with the lesson and the total points.
        /// returns -1 if the specifued course is not found
        /// does not persis data on the database, the calling method must save change to persis it
        /// </summary>
        public static int decrementTotalLessons(carEVAContext context, int courseID, Lesson lesson)
        {
            //when we delete a lesson, it will cascade delete the quizes at database level.
            //so here we must decrease the counter for the quizes
            int totalQuestionsInLesson = lesson.questions.Count();
            int pointsInLesson = 0;
            var questions = context.Questions.Where(q => q.LessonID == lesson.LessonID);
            foreach (Question questionItem in questions)
            {
                pointsInLesson = pointsInLesson + questionItem.points;
            }
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            proxyCourse.totalLessons--;
            proxyCourse.totalQuizes = proxyCourse.totalQuizes - totalQuestionsInLesson;
            proxyCourse.totalPoints = proxyCourse.totalPoints - pointsInLesson;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
        //---------------------------------------------------------------------------------------------
        public static int countDeletedLessons(carEVAContext context, int courseID, Chapter chapter)
        {
            int chapterID = chapter.ChapterID;
            int totalQuizesToRemove = 0;
            int totalPointsToRemove = 0;
            Course proxyCourse = context.Courses.Find(courseID);
            if (proxyCourse == null)
            {
                return -1;
            }
            foreach (Lesson item in chapter.lessons.Where(q => q.ChapterID == chapterID))
            {
                totalQuizesToRemove = totalQuizesToRemove + item.questions.Count;
                foreach(Question questionItem in item.questions.Where(q => q.LessonID == item.LessonID))
                {
                    totalPointsToRemove = totalPointsToRemove + questionItem.points;
                }
                proxyCourse.totalLessons--;
            }
            proxyCourse.totalPoints = proxyCourse.totalPoints - totalPointsToRemove;
            proxyCourse.totalQuizes = proxyCourse.totalQuizes - totalQuizesToRemove;
            context.Entry(proxyCourse).State = EntityState.Modified;
            return 1;
        }
    }
    //*********************************************************************************************

}