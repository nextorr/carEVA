namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removelogInTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.evaLogIns");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.evaLogIns",
                c => new
                    {
                        evaLogInID = c.Int(nullable: false, identity: true),
                        user = c.String(),
                        passKey = c.String(),
                    })
                .PrimaryKey(t => t.evaLogInID);
            
        }
    }
}
