namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userInheritanceFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.evaBaseUsers", "institucionEducativa", c => c.String());
            AddColumn("dbo.evaBaseUsers", "tipoDocumento", c => c.String());
            AddColumn("dbo.evaBaseUsers", "numeroDocumento", c => c.Int());
            AddColumn("dbo.evaBaseUsers", "edad", c => c.Int());
            AddColumn("dbo.evaBaseUsers", "municipio", c => c.String());
            AddColumn("dbo.evaBaseUsers", "gradoEstudio", c => c.String());

            //fix the scorecard fiels so it accepts nulls, as required by the TPH model
            AlterColumn("dbo.evaBaseUsers", "totalEnrollments", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "completedCatalogCourses", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "completedRequiredCourses", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationID", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationAreaID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.evaBaseUsers", "gradoEstudio");
            DropColumn("dbo.evaBaseUsers", "municipio");
            DropColumn("dbo.evaBaseUsers", "edad");
            DropColumn("dbo.evaBaseUsers", "numeroDocumento");
            DropColumn("dbo.evaBaseUsers", "tipoDocumento");
            DropColumn("dbo.evaBaseUsers", "institucionEducativa");

            //fix the scorecard fiels so it accepts nulls, as required by the TPH model
            //reverting the model to previous state
            AlterColumn("dbo.evaBaseUsers", "totalEnrollments", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "completedCatalogCourses", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "completedRequiredCourses", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationID", c => c.Int(nullable: false));
            AlterColumn("dbo.evaBaseUsers", "evaOrganizationAreaID", c => c.Int(nullable: false));
        }
    }
}
