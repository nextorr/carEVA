namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class neworganizationcoursekeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.audiencePerCourses",
                c => new
                    {
                        audiencePerCourseID = c.Int(nullable: false, identity: true),
                        evaOrganizationCourseID = c.Int(nullable: false),
                        evaOrganizationAreaID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.audiencePerCourseID)
                .ForeignKey("dbo.evaOrganizationAreas", t => t.evaOrganizationAreaID, cascadeDelete: true)
                .ForeignKey("dbo.evaOrganizationCourses", t => t.evaOrganizationCourseID)
                .Index(t => t.evaOrganizationCourseID)
                .Index(t => t.evaOrganizationAreaID);
            
            AddColumn("dbo.evaOrganizationAreas", "isEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.evaOrganizationCourses", "originAreaID", c => c.Int(nullable: false, defaultValue:1));
            CreateIndex("dbo.evaOrganizationCourses", "originAreaID");
            AddForeignKey("dbo.evaOrganizationCourses", "originAreaID", "dbo.evaOrganizationAreas", "evaOrganizationAreaID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.audiencePerCourses", "evaOrganizationCourseID", "dbo.evaOrganizationCourses");
            DropForeignKey("dbo.evaOrganizationCourses", "originAreaID", "dbo.evaOrganizationAreas");
            DropForeignKey("dbo.audiencePerCourses", "evaOrganizationAreaID", "dbo.evaOrganizationAreas");
            DropIndex("dbo.evaOrganizationCourses", new[] { "originAreaID" });
            DropIndex("dbo.audiencePerCourses", new[] { "evaOrganizationAreaID" });
            DropIndex("dbo.audiencePerCourses", new[] { "evaOrganizationCourseID" });
            DropColumn("dbo.evaOrganizationCourses", "originAreaID");
            DropColumn("dbo.evaOrganizationAreas", "isEnabled");
            DropTable("dbo.audiencePerCourses");
        }
    }
}
