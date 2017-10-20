namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class areaCodeTypeChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaOrganizationAreas", "areaCode", c => c.String());
            DropColumn("dbo.evaOrganizationAreas", "areaNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaOrganizationAreas", "areaNumber", c => c.Int(nullable: false));
            DropColumn("dbo.evaOrganizationAreas", "areaCode");
        }
    }
}
