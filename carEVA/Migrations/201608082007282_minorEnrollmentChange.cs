namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorEnrollmentChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.evaCourseEnrollments", "completionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.evaCourseEnrollments", "completionDate", c => c.DateTime(nullable: false));
        }
    }
}
