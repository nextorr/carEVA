namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inheritanceScoreCard : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.evaCourseEnrollments", name: "evaUserID", newName: "evaBaseUserID");
            RenameIndex(table: "dbo.evaCourseEnrollments", name: "IX_evaUserID", newName: "IX_evaBaseUserID");
            AddColumn("dbo.evaOrganizationAreas", "externalType", c => c.String());
            AlterColumn("dbo.evaBaseUsers", "totalEnrollments", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "completedCatalogCourses", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "completedRequiredCourses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.evaBaseUsers", "completedRequiredCourses", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "completedCatalogCourses", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "totalEnrollments", c => c.Int());
            DropColumn("dbo.evaOrganizationAreas", "externalType");
            RenameIndex(table: "dbo.evaCourseEnrollments", name: "IX_evaBaseUserID", newName: "IX_evaUserID");
            RenameColumn(table: "dbo.evaCourseEnrollments", name: "evaBaseUserID", newName: "evaUserID");
        }
    }
}
