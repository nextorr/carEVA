using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using carEVA.Models;

namespace carEVA.ViewModels
{
    //used to add some properties to the "My courses" response
    public class userEnrollmets
    {
        public DateTime? dueDate { get; set; }
        public evaCourseEnrollment enrollment { get; set; }
    }
    //use this ViewModel to send information to the APP homepage
    public class userOverviewScore
    {
        public int totalActiveEnrollments { get; set; }
        public int totalCatalogCourses { get; set; }
        public int completedCatalogCourses { get; set; }
        public int totalRequiredCourses { get; set; }
        public int completedRequiredCourses { get; set; }
        
    }
}