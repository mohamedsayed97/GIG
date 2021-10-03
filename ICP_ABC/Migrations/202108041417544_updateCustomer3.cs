namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCustomer3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customer", "CityId", "dbo.City");
            DropForeignKey("dbo.Customer", "NationalityId", "dbo.Nationality");
            DropIndex("dbo.Customer", new[] { "NationalityId" });
            DropIndex("dbo.Customer", new[] { "CityId" });
            AlterColumn("dbo.Customer", "NationalityId", c => c.Int());
            AlterColumn("dbo.Customer", "CityId", c => c.Int());
            AlterColumn("dbo.Customer", "Chk", c => c.Boolean());
            AlterColumn("dbo.Customer", "IssuanceDate", c => c.DateTime());
            CreateIndex("dbo.Customer", "NationalityId");
            CreateIndex("dbo.Customer", "CityId");
            AddForeignKey("dbo.Customer", "CityId", "dbo.City", "CityID");
            AddForeignKey("dbo.Customer", "NationalityId", "dbo.Nationality", "NationalityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customer", "NationalityId", "dbo.Nationality");
            DropForeignKey("dbo.Customer", "CityId", "dbo.City");
            DropIndex("dbo.Customer", new[] { "CityId" });
            DropIndex("dbo.Customer", new[] { "NationalityId" });
            AlterColumn("dbo.Customer", "IssuanceDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customer", "Chk", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Customer", "CityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Customer", "NationalityId", c => c.Int(nullable: false));
            CreateIndex("dbo.Customer", "CityId");
            CreateIndex("dbo.Customer", "NationalityId");
            AddForeignKey("dbo.Customer", "NationalityId", "dbo.Nationality", "NationalityID", cascadeDelete: true);
            AddForeignKey("dbo.Customer", "CityId", "dbo.City", "CityID", cascadeDelete: true);
        }
    }
}
