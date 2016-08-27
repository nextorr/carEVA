namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logmodelmod : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.evaLogs", "MyProperty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaLogs", "MyProperty", c => c.Int(nullable: false));
        }
    }
}
