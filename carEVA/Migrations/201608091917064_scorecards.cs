namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scorecards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "points", c => c.Int(nullable: false));
            AddColumn("dbo.Chapters", "totalPoints", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "totalPoints", c => c.Int(nullable: false));
            AddColumn("dbo.evaCourseEnrollments", "currentScore", c => c.Int(nullable: false));
            AddColumn("dbo.evaCourseEnrollments", "isFinalized", c => c.Boolean(nullable: false));
            AddColumn("dbo.evaUsers", "totalEnrollments", c => c.Int(nullable: false));
            AddColumn("dbo.evaUsers", "completedCatalogCourses", c => c.Int(nullable: false));
            AddColumn("dbo.evaUsers", "completedRequiredCourses", c => c.Int(nullable: false));
            AddColumn("dbo.evaOrganizations", "totalCatalogCourses", c => c.Int(nullable: false));
            AddColumn("dbo.evaOrganizations", "totalRequiredCourses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.evaOrganizations", "totalRequiredCourses");
            DropColumn("dbo.evaOrganizations", "totalCatalogCourses");
            DropColumn("dbo.evaUsers", "completedRequiredCourses");
            DropColumn("dbo.evaUsers", "completedCatalogCourses");
            DropColumn("dbo.evaUsers", "totalEnrollments");
            DropColumn("dbo.evaCourseEnrollments", "isFinalized");
            DropColumn("dbo.evaCourseEnrollments", "currentScore");
            DropColumn("dbo.Courses", "totalPoints");
            DropColumn("dbo.Chapters", "totalPoints");
            DropColumn("dbo.Questions", "points");
        }
    }
}
