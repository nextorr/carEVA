namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userRefinement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaLogIns", "passKey", c => c.String());
            DropColumn("dbo.evaLogIns", "pass");
        }
        
        public override void Down()
        {
            AddColumn("dbo.evaLogIns", "pass", c => c.String());
            DropColumn("dbo.evaLogIns", "passKey");
        }
    }
}
