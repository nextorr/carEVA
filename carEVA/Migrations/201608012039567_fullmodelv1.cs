namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fullmodelv1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.evaImages",
                c => new
                    {
                        evaImageID = c.Int(nullable: false, identity: true),
                        imageName = c.String(),
                        imageStorageName = c.String(),
                        imageURL = c.String(),
                    })
                .PrimaryKey(t => t.evaImageID);
            
            CreateTable(
                "dbo.evaAnswerHistories",
                c => new
                    {
                        evaAnswerHistoryID = c.Int(nullable: false, identity: true),
                        evaQuestionDetailID = c.Int(nullable: false),
                        submitedDate = c.DateTime(nullable: false),
                        selectedAnswerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.evaAnswerHistoryID)
                .ForeignKey("dbo.evaQuestionDetails", t => t.evaQuestionDetailID, cascadeDelete: true)
                .Index(t => t.evaQuestionDetailID);
            
            CreateTable(
                "dbo.evaLessonDetails",
                c => new
                    {
                        evaLessonDetailID = c.Int(nullable: false, identity: true),
                        evaCourseEnrollmentID = c.Int(nullable: false),
                        lessonID = c.Int(nullable: false),
                        isComplete = c.Boolean(nullable: false),
                        percentComplete = c.Int(nullable: false),
                        completionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.evaLessonDetailID)
                .ForeignKey("dbo.evaCourseEnrollments", t => t.evaCourseEnrollmentID, cascadeDelete: true)
                .Index(t => t.evaCourseEnrollmentID);
            
            CreateTable(
                "dbo.evaQuestionDetails",
                c => new
                    {
                        evaQuestionDetailID = c.Int(nullable: false, identity: true),
                        evaCourseEnrollmentID = c.Int(nullable: false),
                        questionID = c.Int(nullable: false),
                        lastGradedAnswerID = c.Int(),
                        isCorrect = c.Boolean(),
                        totalGrongAttempts = c.Int(nullable: false),
                        finalScore = c.Int(),
                        currentMaxScore = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.evaQuestionDetailID)
                .ForeignKey("dbo.evaCourseEnrollments", t => t.evaCourseEnrollmentID, cascadeDelete: true)
                .Index(t => t.evaCourseEnrollmentID);
            
            CreateTable(
                "dbo.evaOrganizationAreas",
                c => new
                    {
                        evaOrganizationAreaID = c.Int(nullable: false, identity: true),
                        evaOrganizationID = c.Int(nullable: false),
                        areaNumber = c.Int(nullable: false),
                        name = c.String(),
                        evaOrganizationCourse_evaOrganizationCourseID = c.Int(),
                    })
                .PrimaryKey(t => t.evaOrganizationAreaID)
                .ForeignKey("dbo.evaOrganizations", t => t.evaOrganizationID, cascadeDelete: true)
                .ForeignKey("dbo.evaOrganizationCourses", t => t.evaOrganizationCourse_evaOrganizationCourseID)
                .Index(t => t.evaOrganizationID)
                .Index(t => t.evaOrganizationCourse_evaOrganizationCourseID);
            
            CreateTable(
                "dbo.evaOrganizations",
                c => new
                    {
                        evaOrganizationID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        domain = c.String(),
                        address = c.String(),
                        phone = c.String(),
                    })
                .PrimaryKey(t => t.evaOrganizationID);
            
            CreateTable(
                "dbo.evaOrganizationCourses",
                c => new
                    {
                        evaOrganizationCourseID = c.Int(nullable: false, identity: true),
                        evaOrganizationID = c.Int(nullable: false),
                        courseID = c.Int(nullable: false),
                        evaAreaOriginID = c.Int(nullable: false),
                        creationDate = c.DateTime(nullable: false),
                        required = c.Boolean(nullable: false),
                        deadline = c.DateTime(nullable: false),
                        originArea_evaOrganizationAreaID = c.Int(),
                    })
                .PrimaryKey(t => t.evaOrganizationCourseID)
                .ForeignKey("dbo.Courses", t => t.courseID, cascadeDelete: true)
                .ForeignKey("dbo.evaOrganizations", t => t.evaOrganizationID, cascadeDelete: true)
                .ForeignKey("dbo.evaOrganizationAreas", t => t.originArea_evaOrganizationAreaID)
                .Index(t => t.evaOrganizationID)
                .Index(t => t.courseID)
                .Index(t => t.originArea_evaOrganizationAreaID);
            
            AddColumn("dbo.Courses", "commitmentHoursPerDay", c => c.Int());
            AddColumn("dbo.Courses", "commitmentHoursTotal", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "commitmentDays", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "totalQuizes", c => c.Int());
            AddColumn("dbo.Courses", "totalLessons", c => c.Int());
            Sql("INSERT INTO dbo.evaImages (imageName, imageStorageName, imageURL) VALUES ('TempModelUpdate', 'tempImage', 'not valid')");
            //since here the database its just created ID 1 is guaranteed to be the temp row
            AddColumn("dbo.Courses", "evaImageID", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.evaCourseEnrollments", "finalScore", c => c.Int());
            CreateIndex("dbo.Courses", "evaImageID");
            AddForeignKey("dbo.Courses", "evaImageID", "dbo.evaImages", "evaImageID", cascadeDelete: true);
            DropColumn("dbo.evaCourseEnrollments", "gradePoints");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaCourseEnrollments", "gradePoints", c => c.Int(nullable: false));
            DropForeignKey("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID", "dbo.evaOrganizationAreas");
            DropForeignKey("dbo.evaOrganizationCourses", "evaOrganizationID", "dbo.evaOrganizations");
            DropForeignKey("dbo.evaOrganizationCourses", "courseID", "dbo.Courses");
            DropForeignKey("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID", "dbo.evaOrganizationCourses");
            DropForeignKey("dbo.evaOrganizationAreas", "evaOrganizationID", "dbo.evaOrganizations");
            DropForeignKey("dbo.evaQuestionDetails", "evaCourseEnrollmentID", "dbo.evaCourseEnrollments");
            DropForeignKey("dbo.evaAnswerHistories", "evaQuestionDetailID", "dbo.evaQuestionDetails");
            DropForeignKey("dbo.evaLessonDetails", "evaCourseEnrollmentID", "dbo.evaCourseEnrollments");
            DropForeignKey("dbo.Courses", "evaImageID", "dbo.evaImages");
            DropIndex("dbo.evaOrganizationCourses", new[] { "originArea_evaOrganizationAreaID" });
            DropIndex("dbo.evaOrganizationCourses", new[] { "courseID" });
            DropIndex("dbo.evaOrganizationCourses", new[] { "evaOrganizationID" });
            DropIndex("dbo.evaOrganizationAreas", new[] { "evaOrganizationCourse_evaOrganizationCourseID" });
            DropIndex("dbo.evaOrganizationAreas", new[] { "evaOrganizationID" });
            DropIndex("dbo.evaQuestionDetails", new[] { "evaCourseEnrollmentID" });
            DropIndex("dbo.evaLessonDetails", new[] { "evaCourseEnrollmentID" });
            DropIndex("dbo.evaAnswerHistories", new[] { "evaQuestionDetailID" });
            DropIndex("dbo.Courses", new[] { "evaImageID" });
            DropColumn("dbo.evaCourseEnrollments", "finalScore");
            DropColumn("dbo.Courses", "evaImageID");
            DropColumn("dbo.Courses", "totalLessons");
            DropColumn("dbo.Courses", "totalQuizes");
            DropColumn("dbo.Courses", "commitmentDays");
            DropColumn("dbo.Courses", "commitmentHoursTotal");
            DropColumn("dbo.Courses", "commitmentHoursPerDay");
            DropTable("dbo.evaOrganizationCourses");
            DropTable("dbo.evaOrganizations");
            DropTable("dbo.evaOrganizationAreas");
            DropTable("dbo.evaQuestionDetails");
            DropTable("dbo.evaLessonDetails");
            DropTable("dbo.evaAnswerHistories");
            DropTable("dbo.evaImages");
        }
    }
}
