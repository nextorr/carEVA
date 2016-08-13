namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lessondetailnulldate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.evaLessonDetails", "completionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.evaLessonDetails", "completionDate", c => c.DateTime(nullable: false));
        }
    }
}
