namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addunique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Funds", "Code", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.ICPrices", "Code", c => c.String(nullable: false, maxLength: 4));
            CreateIndex("dbo.Branches", "Code", unique: true);
            CreateIndex("dbo.Funds", "Code", unique: true);
            CreateIndex("dbo.Currencies", "Code", unique: true);
            CreateIndex("dbo.Sponsors", "Code", unique: true);
            CreateIndex("dbo.Cities", "Code", unique: true);
            CreateIndex("dbo.CustTypes", "Code", unique: true);
            CreateIndex("dbo.Nationalities", "Code", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Nationalities", new[] { "Code" });
            DropIndex("dbo.CustTypes", new[] { "Code" });
            DropIndex("dbo.Cities", new[] { "Code" });
            DropIndex("dbo.Sponsors", new[] { "Code" });
            DropIndex("dbo.Currencies", new[] { "Code" });
            DropIndex("dbo.Funds", new[] { "Code" });
            DropIndex("dbo.Branches", new[] { "Code" });
            AlterColumn("dbo.ICPrices", "Code", c => c.String());
            AlterColumn("dbo.Funds", "Code", c => c.String());
        }
    }
}
