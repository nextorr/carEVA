namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userAreaIntegration : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.evaUsers", "areaCar", "areaCode");
            AddColumn("dbo.evaUsers", "evaOrganizationAreaID", c => c.Int(nullable: false, defaultValue:1));
            CreateIndex("dbo.evaUsers", "evaOrganizationAreaID");
            AddForeignKey("dbo.evaUsers", "evaOrganizationAreaID", "dbo.evaOrganizationAreas", "evaOrganizationAreaID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            RenameColumn("dbo.evaUsers", "areaCode", "areaCar");
            DropForeignKey("dbo.evaUsers", "evaOrganizationAreaID", "dbo.evaOrganizationAreas");
            DropIndex("dbo.evaUsers", new[] { "evaOrganizationAreaID" });
            DropColumn("dbo.evaUsers", "evaOrganizationAreaID");
        }
    }
}
