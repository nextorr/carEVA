namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityUploadSupport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lessons", "lessonType", c => c.Int(nullable: false, defaultValue:0));
            AddColumn("dbo.Lessons", "activityInstructions", c => c.String());
            AddColumn("dbo.evaFiles", "evaLessonDetailID", c => c.Int());
            CreateIndex("dbo.evaFiles", "evaLessonDetailID");
            AddForeignKey("dbo.evaFiles", "evaLessonDetailID", "dbo.evaLessonDetails", "evaLessonDetailID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.evaFiles", "evaLessonDetailID", "dbo.evaLessonDetails");
            DropIndex("dbo.evaFiles", new[] { "evaLessonDetailID" });
            DropColumn("dbo.evaFiles", "evaLessonDetailID");
            DropColumn("dbo.Lessons", "activityInstructions");
            DropColumn("dbo.Lessons", "lessonType");
        }
    }
}
