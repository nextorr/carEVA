namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInheritanceAreaInBase : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationAreaID" });
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationAreaID", c => c.Int(nullable: false));
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationAreaID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationAreaID" });
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationAreaID", c => c.Int());
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationAreaID");
        }
    }
}
