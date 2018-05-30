namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class interactiveActivityURL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lessons", "interactiveActivityURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lessons", "interactiveActivityURL");
        }
    }
}
