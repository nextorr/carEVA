namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class courseModelRefinement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "commitmentHoursPerDay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "commitmentHoursPerDay", c => c.Int());
        }
    }
}
