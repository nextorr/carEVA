namespace carEVA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class defensoresTypeAdjustement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.evaBaseUsers", "tipoDocumento", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "numeroDocumento", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.evaBaseUsers", "numeroDocumento", c => c.Int());
            AlterColumn("dbo.evaBaseUsers", "tipoDocumento", c => c.String());
        }
    }
}
