namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uptodateeed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Currencies", "Code", c => c.String(nullable: false, maxLength: 4));
            CreateIndex("dbo.Currencies", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Currencies", new[] { "Code" });
            AlterColumn("dbo.Currencies", "Code", c => c.String());
        }
    }
}
