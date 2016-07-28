namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AnswerID = c.Int(nullable: false, identity: true),
                        text = c.String(),
                        isCorrect = c.Boolean(nullable: false),
                        QuestionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnswerID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        statement = c.String(),
                        evaType = c.String(),
                        LessonID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionID)
                .ForeignKey("dbo.Lessons", t => t.LessonID, cascadeDelete: true)
                .Index(t => t.LessonID);
            
            CreateTable(
                "dbo.Lessons",
                c => new
                    {
                        LessonID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        videoURL = c.String(),
                        videoName = c.String(),
                        videoStorageName = c.String(),
                        ChapterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LessonID)
                .ForeignKey("dbo.Chapters", t => t.ChapterID, cascadeDelete: true)
                .Index(t => t.ChapterID);
            
            CreateTable(
                "dbo.Chapters",
                c => new
                    {
                        ChapterID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        index = c.Int(nullable: false),
                        CourseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChapterID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseID = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.CourseID);
            
            CreateTable(
                "dbo.evaFiles",
                c => new
                    {
                        evaFileID = c.Int(nullable: false, identity: true),
                        fileName = c.String(),
                        fileStorageName = c.String(),
                        fileURL = c.String(),
                        courseID = c.Int(),
                        chapterID = c.Int(),
                        lessonID = c.Int(),
                    })
                .PrimaryKey(t => t.evaFileID)
                .ForeignKey("dbo.Chapters", t => t.chapterID)
                .ForeignKey("dbo.Courses", t => t.courseID)
                .ForeignKey("dbo.Lessons", t => t.lessonID)
                .Index(t => t.courseID)
                .Index(t => t.chapterID)
                .Index(t => t.lessonID);
            
            CreateTable(
                "dbo.evaTypes",
                c => new
                    {
                        evaTypeID = c.Int(nullable: false, identity: true),
                        answerType = c.String(),
                    })
                .PrimaryKey(t => t.evaTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "LessonID", "dbo.Lessons");
            DropForeignKey("dbo.Lessons", "ChapterID", "dbo.Chapters");
            DropForeignKey("dbo.evaFiles", "lessonID", "dbo.Lessons");
            DropForeignKey("dbo.evaFiles", "courseID", "dbo.Courses");
            DropForeignKey("dbo.evaFiles", "chapterID", "dbo.Chapters");
            DropForeignKey("dbo.Chapters", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.Answers", "QuestionID", "dbo.Questions");
            DropIndex("dbo.evaFiles", new[] { "lessonID" });
            DropIndex("dbo.evaFiles", new[] { "chapterID" });
            DropIndex("dbo.evaFiles", new[] { "courseID" });
            DropIndex("dbo.Chapters", new[] { "CourseID" });
            DropIndex("dbo.Lessons", new[] { "ChapterID" });
            DropIndex("dbo.Questions", new[] { "LessonID" });
            DropIndex("dbo.Answers", new[] { "QuestionID" });
            DropTable("dbo.evaTypes");
            DropTable("dbo.evaFiles");
            DropTable("dbo.Courses");
            DropTable("dbo.Chapters");
            DropTable("dbo.Lessons");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}
