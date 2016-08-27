namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.evaLogs",
                c => new
                    {
                        evaLogID = c.Int(nullable: false, identity: true),
                        MyProperty = c.Int(nullable: false),
                        caller = c.String(),
                        message = c.String(),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.evaLogID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.evaLogs");
        }
    }
}
