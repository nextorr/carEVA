namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class courseOwner : DbMigration
    {
        public override void Up()
        {
            //add a default value to point to some ID on the instructor table.
            AddColumn("dbo.Courses", "evaInstructorID", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Courses", "evaInstructorID");
            AddForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaInstructors", "evaInstructorID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaInstructors");
            DropIndex("dbo.Courses", new[] { "evaInstructorID" });
            DropColumn("dbo.Courses", "evaInstructorID");
        }
    }
}
