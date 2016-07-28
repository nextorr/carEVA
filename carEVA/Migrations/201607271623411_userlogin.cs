namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userlogin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.evaLogIns",
                c => new
                    {
                        evaLogInID = c.Int(nullable: false, identity: true),
                        user = c.String(),
                        pass = c.String(),
                    })
                .PrimaryKey(t => t.evaLogInID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.evaLogIns");
        }
    }
}
