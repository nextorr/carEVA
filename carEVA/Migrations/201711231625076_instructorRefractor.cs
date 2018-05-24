namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instructorRefractor : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Courses", name: "evaInstructorID", newName: "createdByID");
            RenameIndex(table: "dbo.Courses", name: "IX_evaInstructorID", newName: "IX_createdByID");
            CreateTable(
                "dbo.organizationCourseColaborators",
                c => new
                    {
                        Colaborator_evaOrganizationCourseID = c.Int(nullable: false),
                        ColaboratorOf_evaInstructorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Colaborator_evaOrganizationCourseID, t.ColaboratorOf_evaInstructorID })
                .ForeignKey("dbo.evaBaseUsers", t => t.Colaborator_evaOrganizationCourseID, cascadeDelete: false)
                .ForeignKey("dbo.evaOrganizationCourses", t => t.ColaboratorOf_evaInstructorID, cascadeDelete: false)
                .Index(t => t.Colaborator_evaOrganizationCourseID)
                .Index(t => t.ColaboratorOf_evaInstructorID);
            
            AddColumn("dbo.evaOrganizationCourses", "evaInstructorID", c => c.Int(nullable: false, defaultValue: 119));
            CreateIndex("dbo.evaOrganizationCourses", "evaInstructorID");
            AddForeignKey("dbo.evaOrganizationCourses", "evaInstructorID", "dbo.evaBaseUsers", "ID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.organizationCourseColaborators", "ColaboratorOf_evaInstructorID", "dbo.evaOrganizationCourses");
            DropForeignKey("dbo.organizationCourseColaborators", "Colaborator_evaOrganizationCourseID", "dbo.evaBaseUsers");
            DropForeignKey("dbo.evaOrganizationCourses", "evaInstructorID", "dbo.evaBaseUsers");
            DropIndex("dbo.organizationCourseColaborators", new[] { "ColaboratorOf_evaInstructorID" });
            DropIndex("dbo.organizationCourseColaborators", new[] { "Colaborator_evaOrganizationCourseID" });
            DropIndex("dbo.evaOrganizationCourses", new[] { "evaInstructorID" });
            DropColumn("dbo.evaOrganizationCourses", "evaInstructorID");
            DropTable("dbo.organizationCourseColaborators");
            RenameIndex(table: "dbo.Courses", name: "IX_createdByID", newName: "IX_evaInstructorID");
            RenameColumn(table: "dbo.Courses", name: "createdByID", newName: "evaInstructorID");
        }
    }
}
