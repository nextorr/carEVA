namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorchangeCourse : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "title", c => c.String(nullable: false));
            AlterColumn("dbo.Courses", "description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "description", c => c.String());
            AlterColumn("dbo.Courses", "title", c => c.String());
        }
    }
}
