namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AreaUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaOrganizationAreas", "nameAbreviation", c => c.String());
            AddColumn("dbo.evaOrganizationAreas", "isExternal", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.evaOrganizationAreas", "isExternal");
            DropColumn("dbo.evaOrganizationAreas", "nameAbreviation");
        }
    }
}
