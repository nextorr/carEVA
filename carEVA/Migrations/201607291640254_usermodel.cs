namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usermodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.evaCourseEnrollments",
                c => new
                    {
                        evaCourseEnrollmentID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        evaUserID = c.Int(nullable: false),
                        completedLessons = c.Int(nullable: false),
                        gradePoints = c.Int(nullable: false),
                        EnrollmentDate = c.DateTime(nullable: false),
                        completionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.evaCourseEnrollmentID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.evaUsers", t => t.evaUserID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.evaUserID);
            
            CreateTable(
                "dbo.evaUsers",
                c => new
                    {
                        evaUserID = c.Int(nullable: false, identity: true),
                        userName = c.String(),
                        email = c.String(),
                        fullName = c.String(),
                        aspnetUserID = c.String(),
                        areaCar = c.String(),
                        gender = c.String(),
                        publicKey = c.String(),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.evaUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.evaCourseEnrollments", "evaUserID", "dbo.evaUsers");
            DropForeignKey("dbo.evaCourseEnrollments", "CourseID", "dbo.Courses");
            DropIndex("dbo.evaCourseEnrollments", new[] { "evaUserID" });
            DropIndex("dbo.evaCourseEnrollments", new[] { "CourseID" });
            DropTable("dbo.evaUsers");
            DropTable("dbo.evaCourseEnrollments");
        }
    }
}
