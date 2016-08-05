namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCourseandOrgAdjust : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaUsers", "evaOrganizationID", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.evaOrganizations", "nameShort", c => c.String());
            AddColumn("dbo.evaOrganizations", "nameAlternative", c => c.String());
            AddColumn("dbo.evaOrganizations", "nameAbbreviation", c => c.String());
            AddColumn("dbo.evaOrganizations", "slogan", c => c.String());
            AddColumn("dbo.evaOrganizations", "mainOfficeCountry", c => c.String());
            AddColumn("dbo.evaOrganizations", "mainOfficeCity", c => c.String());
            AlterColumn("dbo.evaOrganizations", "name", c => c.String(nullable: false, maxLength: 450));
            AlterColumn("dbo.evaOrganizations", "domain", c => c.String(nullable: false, maxLength: 450));
            CreateIndex("dbo.evaUsers", "evaOrganizationID");
            CreateIndex("dbo.evaOrganizations", "name", unique: true);
            CreateIndex("dbo.evaOrganizations", "domain", unique: true);
            AddForeignKey("dbo.evaUsers", "evaOrganizationID", "dbo.evaOrganizations", "evaOrganizationID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.evaUsers", "evaOrganizationID", "dbo.evaOrganizations");
            DropIndex("dbo.evaOrganizations", new[] { "domain" });
            DropIndex("dbo.evaOrganizations", new[] { "name" });
            DropIndex("dbo.evaUsers", new[] { "evaOrganizationID" });
            AlterColumn("dbo.evaOrganizations", "domain", c => c.String());
            AlterColumn("dbo.evaOrganizations", "name", c => c.String());
            DropColumn("dbo.evaOrganizations", "mainOfficeCity");
            DropColumn("dbo.evaOrganizations", "mainOfficeCountry");
            DropColumn("dbo.evaOrganizations", "slogan");
            DropColumn("dbo.evaOrganizations", "nameAbbreviation");
            DropColumn("dbo.evaOrganizations", "nameAlternative");
            DropColumn("dbo.evaOrganizations", "nameShort");
            DropColumn("dbo.evaUsers", "evaOrganizationID");
        }
    }
}
