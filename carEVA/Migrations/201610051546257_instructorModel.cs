namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instructorModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.evaInstructors",
                c => new
                    {
                        evaInstructorID = c.Int(nullable: false, identity: true),
                        userName = c.String(),
                        email = c.String(),
                        altEmail = c.String(),
                        fullName = c.String(),
                        mobileNumber = c.String(),
                        aspnetUserID = c.String(),
                        gender = c.String(),
                        isActive = c.Boolean(nullable: false),
                        evaOrganizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.evaInstructorID)
                .ForeignKey("dbo.evaOrganizations", t => t.evaOrganizationID, cascadeDelete: false)
                .Index(t => t.evaOrganizationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.evaInstructors", "evaOrganizationID", "dbo.evaOrganizations");
            DropIndex("dbo.evaInstructors", new[] { "evaOrganizationID" });
            DropTable("dbo.evaInstructors");
        }
    }
}
