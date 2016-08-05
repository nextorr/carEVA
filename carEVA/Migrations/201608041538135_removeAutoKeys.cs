namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeAutoKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID", "dbo.evaOrganizationCourses");
            DropForeignKey("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID", "dbo.evaOrganizationAreas");
            DropIndex("dbo.evaOrganizationAreas", new[] { "evaOrganizationCourse_evaOrganizationCourseID" });
            DropIndex("dbo.evaOrganizationCourses", new[] { "originArea_evaOrganizationAreaID" });
            AlterColumn("dbo.evaOrganizationCourses", "deadline", c => c.DateTime());
            DropColumn("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID");
            DropColumn("dbo.evaOrganizationCourses", "evaAreaOriginID");
            DropColumn("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID", c => c.Int());
            AddColumn("dbo.evaOrganizationCourses", "evaAreaOriginID", c => c.Int(nullable: false));
            AddColumn("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID", c => c.Int());
            AlterColumn("dbo.evaOrganizationCourses", "deadline", c => c.DateTime(nullable: false));
            CreateIndex("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID");
            CreateIndex("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID");
            AddForeignKey("dbo.evaOrganizationCourses", "originArea_evaOrganizationAreaID", "dbo.evaOrganizationAreas", "evaOrganizationAreaID");
            AddForeignKey("dbo.evaOrganizationAreas", "evaOrganizationCourse_evaOrganizationCourseID", "dbo.evaOrganizationCourses", "evaOrganizationCourseID");
        }
    }
}
