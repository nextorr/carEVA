namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coursePermissions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.audiencePerCourses", "evaOrganizationCourseID", "dbo.evaOrganizationCourses");
            RenameTable(name: "dbo.audiencePerCourses", newName: "evaOrgCourseAreaPermissions");
            DropPrimaryKey("dbo.evaOrgCourseAreaPermissions");
            DropColumn("dbo.evaOrgCourseAreaPermissions", "audiencePerCourseID");
            AddColumn("dbo.evaOrgCourseAreaPermissions", "evaOrgCourseAreaPermissionsID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.evaOrgCourseAreaPermissions", "permissionLevel", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.evaOrgCourseAreaPermissions", "evaOrgCourseAreaPermissionsID");
            AddForeignKey("dbo.evaOrgCourseAreaPermissions", "evaOrganizationCourseID", "dbo.evaOrganizationCourses", "evaOrganizationCourseID", cascadeDelete: false);
            
        }
        
        public override void Down()
        {
            
            DropForeignKey("dbo.evaOrgCourseAreaPermissions", "evaOrganizationCourseID", "dbo.evaOrganizationCourses");
            DropPrimaryKey("dbo.evaOrgCourseAreaPermissions");
            DropColumn("dbo.evaOrgCourseAreaPermissions", "permissionLevel");
            DropColumn("dbo.evaOrgCourseAreaPermissions", "evaOrgCourseAreaPermissionsID");
            AddColumn("dbo.evaOrgCourseAreaPermissions", "audiencePerCourseID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.evaOrgCourseAreaPermissions", "audiencePerCourseID");
            RenameTable(name: "dbo.evaOrgCourseAreaPermissions", newName: "audiencePerCourses");
            AddForeignKey("dbo.audiencePerCourses", "evaOrganizationCourseID", "dbo.evaOrganizationCourses", "evaOrganizationCourseID");
        }
    }
}
