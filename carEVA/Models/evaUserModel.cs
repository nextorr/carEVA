using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace carEVA.Models
{
    //use this to define the genders. 
    //Cant use a enum because integration with SIDCAR.
    static public class genderType
    {
        static private readonly string[] genderDefinition = {"", "M", "F" };
        static public List<string> genderList { get { return genderDefinition.ToList(); } }
    }
    public class evaLogIn
    {
        public string user { get; set; }
        public string domain { get; set; }
        public string passKey { get; set; }
        public string userAndDomain
        {
            get
            {
                return user + "@" + domain;
            }
        }
        public bool containsValidInfo()
        {
            bool valid = true;
            if (user.Length <= 0 || domain.Length <= 0 || passKey.Length <= 0)
            {
                valid = false;
            }
            return valid;
        }
    }
    //user inheritance using table-per-hierarchy (TPC) pattern, 
    //see https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-inheritance-with-the-entity-framework-in-an-asp-net-mvc-application
    public abstract class evaBaseUser
    {
        public int ID { get; set; }
        [DisplayName("Nombre de Usuario")]
        public string userName { get; set; }
        //the username is in the email format,but we need an additional email field to 
        //allow for user alternate email address.
        [DisplayName("Correo Electronico")]
        public string email { get; set; }
        [DisplayName("Nombre Completo")]
        public string fullName { get; set; }
        public string aspnetUserID { get; set; }
        public string areaCode { get; set; }
        [DisplayName("Genero")]
        public string gender { get; set; }
        public string publicKey { get; set; }
        public bool isActive { get; set; }
        //use this data to keep a scoreCard of the student
        public int totalEnrollments { get; set; }
        public int completedCatalogCourses { get; set; }
        public int completedRequiredCourses { get; set; }
        //TODO: some create some properties to get the first and last name
        //the user must belong to an area
        //......................Navigation Properties..............................
        //the user can be associated with only one Organization
        public int evaOrganizationID { get; set; }
        [ForeignKey("evaOrganizationID")]
        public virtual evaOrganization organization { get; set; }
        public int evaOrganizationAreaID { get; set; }
        public virtual evaOrganizationArea organizationArea { get; set; }
        public virtual ICollection<evaCourseEnrollment> CourseEnrollments { get; set; }
        //---------------------Abstract properties----------------------
        [JsonIgnore]
        public abstract string getIndexViewName { get;}
        [JsonIgnore]
        public abstract string getCreateActionName { get;}
        [JsonIgnore]
        public abstract string getEditViewName { get; }
        public virtual string registerUserControllerName()
        {
            throw new NotImplementedException();
        }

    }
    public class evaUser : evaBaseUser
    {
        //----------------------implementing abstract properties--------------------
        public override string getIndexViewName
        {
            get
            {
                return "_userList";
            }
        }
        public override string getCreateActionName
        {
            get
            {
                return "evaUserCreate";
            }
        }
        public override string getEditViewName
        {
            get
            {
                return "evaUserEdit";
            }
        }
    }

    public class evaCourseEnrollment
    {
        public int evaCourseEnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int evaBaseUserID { get; set; }
        //this is to show the user a quick view of the completed lessons like 3/10
        //as 20 sept, this represent the total viewed lessons
        public int completedLessons { get; set; }
        public int? finalScore { get; set; }
        public int currentScore { get; set; }
        public bool isFinalized { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? completionDate { get; set; }
        //------------------------------
        public virtual Course Course { get; set; }
        [ForeignKey("evaBaseUserID")]
        public virtual evaBaseUser evaUser { get; set; }
        //this gives me the lesson detail per user
        public virtual ICollection<evaLessonDetail> lessonDetail { get; set; }
        
    }
    public class evaLessonDetail
    {
        public int evaLessonDetailID { get; set; }
        public int evaCourseEnrollmentID { get; set; }
        public int lessonID { get; set; }
        public bool viewed { get; set; } //indicate if the associated video has been totally viewed.
        public bool passed { get; set; } //indicate if the associated Quiz has been passed.
        [DefaultValue(0)]
        public int currentTotalGrade { get; set; } //adding the grade for every question in the lesson.
        public DateTime? completionDate { get; set; }
        //navigation properties
        public virtual evaCourseEnrollment courseEnrollment { get; set; }
        public virtual ICollection<evaQuestionDetail> questionDetail { get; set; }

    }
    public class evaQuestionDetail
    {
        public int evaQuestionDetailID { get; set; }
        public int? lastGradedAnswerID { get; set; }
        public bool? isCorrect { get; set; } //null at start, so the answer is not counted as correct or incorrect.
        public int totalGrongAttempts { get; set; }
        public int? finalScore { get; set; }
        public int currentMaxScore { get; set; }
        //navigation properties
        public int questionID { get; set; }
        public int evaLessonDetailID { get; set; }
        public virtual evaLessonDetail evaLessonDetail { get; set; }
        public virtual ICollection<evaAnswerHistory> answerHistory { get; set; }
    }
    public class evaAnswerHistory
    {
        public int evaAnswerHistoryID { get; set; }
        public DateTime submitedDate { get; set; }
        public int selectedAnswerID { get; set; }
        public int maxScore { get; set; }
        public bool isCorrect { get; set; }
        public int score { get; set; }
        //......................Navigation Properties..............................
        public int evaQuestionDetailID { get; set; }
        public virtual evaQuestionDetail questionDetail { get; set; }
    }
}