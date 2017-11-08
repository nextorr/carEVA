namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class municipios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.municipios",
                c => new
                    {
                        municipioID = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        evaOrganizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.municipioID)
                .ForeignKey("dbo.evaOrganizations", t => t.evaOrganizationID, cascadeDelete: true)
                .Index(t => t.evaOrganizationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.municipios", "evaOrganizationID", "dbo.evaOrganizations");
            DropIndex("dbo.municipios", new[] { "evaOrganizationID" });
            DropTable("dbo.municipios");
        }
    }
}
