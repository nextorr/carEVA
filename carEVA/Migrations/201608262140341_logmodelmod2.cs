namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logmodelmod2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaLogs", "level", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.evaLogs", "level");
        }
    }
}
