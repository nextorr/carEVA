namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInheritance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaInstructors");
            DropIndex("dbo.Courses", new[] { "evaInstructorID" });

            RenameTable(name: "dbo.evaUsers", newName: "evaBaseUsers");
            DropForeignKey("dbo.evaInstructors", "evaOrganizationID", "dbo.evaOrganizations");
            DropForeignKey("dbo.evaCourseEnrollments", "evaUserID", "dbo.evaUsers");
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationAreaID" });
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationID" });
            DropIndex("dbo.evaInstructors", new[] { "evaOrganizationID" });
            RenameColumn("dbo.evaBaseUsers", name: "evaUserID", newName: "ID");
            //DropPrimaryKey("dbo.evaBaseUsers");
            //AddColumn("dbo.evaBaseUsers", "ID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.evaBaseUsers", "altEmail", c => c.String());
            AddColumn("dbo.evaBaseUsers", "mobileNumber", c => c.String());
            AddColumn("dbo.evaBaseUsers", "evaInstructorOrganizationID", c => c.Int());
            AddColumn("dbo.evaBaseUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            //AddPrimaryKey("dbo.evaBaseUsers", "ID");
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationAreaID");
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationID");
            CreateIndex("dbo.evaBaseUsers", "evaInstructorOrganizationID");
            AddForeignKey("dbo.evaBaseUsers", "evaInstructorOrganizationID", "dbo.evaOrganizations", "evaOrganizationID", cascadeDelete: false);
            AddForeignKey("dbo.evaCourseEnrollments", "evaUserID", "dbo.evaBaseUsers", "ID", cascadeDelete: true);
            //DropColumn("dbo.evaBaseUsers", "evaUserID");

            //copy the data from the instructors table to the baseUsers table
            AddColumn("dbo.evaBaseUsers", "tempInstructorID", c => c.Int(nullable: true));
            Sql("INSERT INTO dbo.evaBaseUsers (userName, email, fullName, aspnetUserID, areaCode, gender, publicKey, isActive, altEmail, mobileNumber, evaInstructorOrganizationID, tempInstructorID, Discriminator) SELECT userName, email, fullName, aspnetUserID, null AS areaCode, gender, null AS publicKey, isActive, altEmail, mobileNumber, evaOrganizationID AS evaInstructorOrganizationID, evaInstructorID AS tempInstructorID, 'evaInstructor' AS Discriminator FROM dbo.evaInstructors");
            //fix existing relations with the course table
            Sql("UPDATE dbo.Courses SET evaInstructorID = (SELECT ID FROM dbo.evaBaseUsers WHERE tempInstructorID = Courses.evaInstructorID AND Discriminator = 'evaInstructor')");
            //remove temp key
            DropColumn("dbo.evaBaseUsers", "tempInstructorID");


            CreateIndex("dbo.Courses", "evaInstructorID");
            AddForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaBaseUsers", "ID", cascadeDelete: false);


            DropTable("dbo.evaInstructors");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.evaInstructors",
                c => new
                    {
                        evaInstructorID = c.Int(nullable: false, identity: true),
                        userName = c.String(),
                        email = c.String(),
                        altEmail = c.String(),
                        fullName = c.String(),
                        mobileNumber = c.String(),
                        aspnetUserID = c.String(),
                        gender = c.String(),
                        isActive = c.Boolean(nullable: false),
                        evaOrganizationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.evaInstructorID);

            //add the data from evaBaseUsers to this table.
            Sql("INSERT INTO dbo.evaInstructors (userName, email, altEmail, fullName, mobileNumber, aspnetUserID,gender, isActive, evaOrganizationID ) SELECT userName, email, altEmail, fullName, mobileNumber, aspnetUserID, gender, isActive, evaInstructorOrganizationID FROM dbo.evaBaseUsers WHERE Discriminator = 'evaInstructor'");
            //then delete all evaInstructors from this table
            Sql("DELETE FROM dbo.evaBaseUsers WHERE Discriminator = 'evaInstructor'");
            
            //AddColumn("dbo.evaBaseUsers", "evaUserID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaBaseUsers");
            DropForeignKey("dbo.evaCourseEnrollments", "evaUserID", "dbo.evaBaseUsers");
            DropForeignKey("dbo.evaBaseUsers", "evaInstructorOrganizationID", "dbo.evaOrganizations");
            DropIndex("dbo.evaBaseUsers", new[] { "evaInstructorOrganizationID" });
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationID" });
            DropIndex("dbo.evaBaseUsers", new[] { "evaOrganizationAreaID" });
            DropIndex("dbo.Courses", new[] { "evaInstructorID" });
            //DropPrimaryKey("dbo.evaBaseUsers");
            DropColumn("dbo.evaBaseUsers", "Discriminator");
            DropColumn("dbo.evaBaseUsers", "evaInstructorOrganizationID");
            DropColumn("dbo.evaBaseUsers", "mobileNumber");
            DropColumn("dbo.evaBaseUsers", "altEmail");
            //DropColumn("dbo.evaBaseUsers", "ID");
            //AddPrimaryKey("dbo.evaBaseUsers", "evaUserID");
            RenameColumn("dbo.evaBaseUsers", name: "ID", newName: "evaUserID");
            CreateIndex("dbo.evaInstructors", "evaOrganizationID");
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationID");
            CreateIndex("dbo.evaBaseUsers", "evaOrganizationAreaID");
            CreateIndex("dbo.Courses", "evaInstructorID");
            AddForeignKey("dbo.evaCourseEnrollments", "evaUserID", "dbo.evaUsers", "evaUserID", cascadeDelete: true);
            AddForeignKey("dbo.evaInstructors", "evaOrganizationID", "dbo.evaOrganizations", "evaOrganizationID", cascadeDelete: false);
            AddForeignKey("dbo.Courses", "evaInstructorID", "dbo.evaInstructors", "evaInstructorID", cascadeDelete: false);
            RenameTable(name: "dbo.evaBaseUsers", newName: "evaUsers");
        }
    }
}
