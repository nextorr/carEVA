namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInheritanceOrganizationInBase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.evaBaseUsers", "evaInstructorOrganizationID", "dbo.evaOrganizations");
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationID" });
            DropIndex("dbo.evaBaseUsers", new[] { "evaInstructorOrganizationID" });
            DropColumn("dbo.evaBaseUsers", "evaInstructorOrganizationID");
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationID", c => c.Int(nullable: false));
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationID" });
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationID", c => c.Int());
            AddColumn("dbo.evaBaseUsers", "evaInstructorOrganizationID", c => c.Int(nullable:true));
            CreateIndex("dbo.evaBaseUsers", "evaInstructorOrganizationID");
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationID");

            //update the evaInstructorOrganization accordingly
            Sql("UPDATE dbo.evaBaseUsers SET evaInstructorOrganizationID = 1 where Discriminator = 'evaInstructor'");

            AddForeignKey("dbo.evaBaseUsers", "evaInstructorOrganizationID", "dbo.evaOrganizations", "evaOrganizationID", cascadeDelete: false);
        }
    }
}
