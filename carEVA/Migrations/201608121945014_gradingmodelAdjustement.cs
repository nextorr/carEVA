namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gradingmodelAdjustement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.evaQuestionDetails", "evaCourseEnrollmentID", "dbo.evaCourseEnrollments");
            DropIndex("dbo.evaQuestionDetails", new[] { "evaCourseEnrollmentID" });
            RenameColumn(table: "dbo.evaQuestionDetails", name: "evaCourseEnrollmentID", newName: "evaLessonDetail_evaCourseEnrollmentID");
            AddColumn("dbo.evaLessonDetails", "viewed", c => c.Boolean(nullable: false));
            AddColumn("dbo.evaLessonDetails", "passed", c => c.Boolean(nullable: false));
            AddColumn("dbo.evaLessonDetails", "currentTotalGrade", c => c.Int(nullable: false));
            AddColumn("dbo.evaQuestionDetails", "evaLessonDetailID", c => c.Int(nullable: false));
            AddColumn("dbo.evaAnswerHistories", "maxScore", c => c.Int(nullable: false));
            AddColumn("dbo.evaAnswerHistories", "isCorrect", c => c.Boolean(nullable: false));
            AddColumn("dbo.evaAnswerHistories", "score", c => c.Int(nullable: false));
            AlterColumn("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", c => c.Int());
            CreateIndex("dbo.evaQuestionDetails", "evaLessonDetailID");
            CreateIndex("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID");
            AddForeignKey("dbo.evaQuestionDetails", "evaLessonDetailID", "dbo.evaLessonDetails", "evaLessonDetailID", cascadeDelete: true);
            AddForeignKey("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", "dbo.evaCourseEnrollments", "evaCourseEnrollmentID");
            DropColumn("dbo.evaLessonDetails", "isComplete");
            DropColumn("dbo.evaLessonDetails", "percentComplete");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaLessonDetails", "percentComplete", c => c.Int(nullable: false));
            AddColumn("dbo.evaLessonDetails", "isComplete", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", "dbo.evaCourseEnrollments");
            DropForeignKey("dbo.evaQuestionDetails", "evaLessonDetailID", "dbo.evaLessonDetails");
            DropIndex("dbo.evaQuestionDetails", new[] { "evaLessonDetail_evaCourseEnrollmentID" });
            DropIndex("dbo.evaQuestionDetails", new[] { "evaLessonDetailID" });
            AlterColumn("dbo.evaQuestionDetails", "evaLessonDetail_evaCourseEnrollmentID", c => c.Int(nullable: false));
            DropColumn("dbo.evaAnswerHistories", "score");
            DropColumn("dbo.evaAnswerHistories", "isCorrect");
            DropColumn("dbo.evaAnswerHistories", "maxScore");
            DropColumn("dbo.evaQuestionDetails", "evaLessonDetailID");
            DropColumn("dbo.evaLessonDetails", "currentTotalGrade");
            DropColumn("dbo.evaLessonDetails", "passed");
            DropColumn("dbo.evaLessonDetails", "viewed");
            RenameColumn(table: "dbo.evaQuestionDetails", name: "evaLessonDetail_evaCourseEnrollmentID", newName: "evaCourseEnrollmentID");
            CreateIndex("dbo.evaQuestionDetails", "evaCourseEnrollmentID");
            AddForeignKey("dbo.evaQuestionDetails", "evaCourseEnrollmentID", "dbo.evaCourseEnrollments", "evaCourseEnrollmentID", cascadeDelete: true);
        }
    }
}
