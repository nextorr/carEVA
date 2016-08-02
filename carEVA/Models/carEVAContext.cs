using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace carEVA.Models
{
    public class carEVAContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public carEVAContext() : base("name=carEVAContext")
        {
        }

        public System.Data.Entity.DbSet<carEVA.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<carEVA.Models.Chapter> Chapters { get; set; }

        public System.Data.Entity.DbSet<carEVA.Models.Lesson> Lessons { get; set; }

        public System.Data.Entity.DbSet<carEVA.Models.Question> Questions { get; set; }

        public System.Data.Entity.DbSet<carEVA.Models.Answer> Answers { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaType> evaTypes { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaFile> Files { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaLogIn> evaLogIns { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaUser> evaUsers { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaCourseEnrollment> evaCourseEnrollments { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaLessonDetail> evaLessonDetails { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaQuestionDetail> evaQuestionDetails { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaAnswerHistory> evaAnswerHistorys { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaImage> evaImages { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaOrganization> evaOrganizations { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaOrganizationArea> evaOrganizationAreas { get; set; }
        public System.Data.Entity.DbSet<carEVA.Models.evaOrganizationCourse> evaOrganizationCourses { get; set; }

    }
}
