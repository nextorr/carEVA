namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class questionDetailCorrection : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", "dbo.evaCourseEnrollments");
            DropIndex("dbo.evaQuestionDetails", new[] { "evaLessonDetail_evaCourseEnrollmentID" });
            DropColumn("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", c => c.Int(nullable: true));
            CreateIndex("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID");
            AddForeignKey("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", "dbo.evaCourseEnrollments", "evaCourseEnrollmentID");
        }
    }
}
