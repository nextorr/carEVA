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
        public string pass { get; set; }
    }
    public class evaUser
    {
        public int evaUserID { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public string aspnetUserID { get; set; }
        public string areaCar { get; set; }
        public string gender { get; set; }

    }

    public class evaEnrollment
    {
        public int evaEnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int evaUserID { get; set; }
        public int completedLessons { get; set; }
        public int gradePoints { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime completionDate { get; set; }
        public virtual Course Course { get; set; }
        public virtual evaUser evaUser { get; set; }
        


    }
}