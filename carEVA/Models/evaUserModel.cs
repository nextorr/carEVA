using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class evaLogIn
    {
        public int evaLogInID { get; set; }
        public string user { get; set; }
        public string passKey { get; set; }
    }
    public class evaUser
    {
        public int evaUserID { get; set; }
        public string userName { get; set; }
        //the username is in the email format,but we need an additional email field to 
        //allow for user alternate email address.
        public string email { get; set; }
        public string fullName { get; set; }
        public string aspnetUserID { get; set; }
        public string areaCar { get; set; }
        public string gender { get; set; }
        public string publicKey { get; set; }
        public bool isActive { get; set; }
        //a user can be enrrolled in multiple courses
        public virtual ICollection<evaCourseEnrollment> CourseEnrollments { get; set; }
        //the user can be associated with only one Organization
        public int evaOrganizationID { get; set; }
        public virtual evaOrganization organization { get; set; }

    }

    public class evaCourseEnrollment
    {
        public int evaCourseEnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int evaUserID { get; set; }
        //this is to show the user a quick view of the completed lessons like 3/10
        public int completedLessons { get; set; }
        public int? finalScore { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime completionDate { get; set; }
        public virtual Course Course { get; set; }
        public virtual evaUser evaUser { get; set; }
        //this gives me the lesson detail per user
        public virtual ICollection<evaLessonDetail> lessonDetail { get; set; }
        public virtual ICollection<evaQuestionDetail> questionDetail { get; set; }
    }
    public class evaLessonDetail
    {
        public int evaLessonDetailID { get; set; }
        public int evaCourseEnrollmentID { get; set; }
        public int lessonID { get; set; }
        public bool isComplete { get; set; }
        public int percentComplete { get; set; }
        public DateTime completionDate { get; set; }

    }
    public class evaQuestionDetail
    {
        public int evaQuestionDetailID { get; set; }
        public int evaCourseEnrollmentID { get; set; }
        public int questionID { get; set; }
        public int? lastGradedAnswerID { get; set; }
        public bool? isCorrect { get; set; }
        public int totalGrongAttempts { get; set; }
        public int? finalScore { get; set; }
        public int currentMaxScore { get; set; }
        public virtual ICollection<evaAnswerHistory> answerHistory{ get; set; }
    }
    public class evaAnswerHistory
    {
        public int evaAnswerHistoryID { get; set; }
        public int evaQuestionDetailID { get; set; }
        public DateTime submitedDate { get; set; }
        public int selectedAnswerID { get; set; }
    }
}