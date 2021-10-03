namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recreatecustomer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Customers", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Customers", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Customers", "CustTypeId", "dbo.CustTypes");
            DropForeignKey("dbo.Customers", "identityType", "dbo.UserIdentityTypes");
            DropForeignKey("dbo.Customers", "NationalityId", "dbo.Nationalities");
            DropIndex("dbo.Customers", new[] { "AccountNO" });
            DropIndex("dbo.Customers", new[] { "CustTypeId" });
            DropIndex("dbo.Customers", new[] { "NationalityId" });
            DropIndex("dbo.Customers", new[] { "BranchId" });
            DropIndex("dbo.Customers", new[] { "CityId" });
            DropIndex("dbo.Customers", new[] { "identityType" });
            DropIndex("dbo.Customers", new[] { "UserID" });
            DropTable("dbo.Customers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        AccountNO = c.Int(nullable: false),
                        EnName = c.String(nullable: false),
                        ArName = c.String(),
                        EnAddress1 = c.String(nullable: false),
                        EnAddress2 = c.String(),
                        ArAddress1 = c.String(nullable: false),
                        ArAddress2 = c.String(),
                        Email1 = c.String(),
                        Email2 = c.String(),
                        Email3 = c.String(),
                        Email4 = c.String(),
                        CRNumber = c.String(),
                        Sector = c.String(),
                        IssuanceDate = c.DateTime(nullable: false),
                        CustTypeId = c.Int(nullable: false),
                        NationalityId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        IdNumber = c.String(nullable: false),
                        identityType = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Boolean(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateIndex("dbo.Customers", "UserID");
            CreateIndex("dbo.Customers", "identityType");
            CreateIndex("dbo.Customers", "CityId");
            CreateIndex("dbo.Customers", "BranchId");
            CreateIndex("dbo.Customers", "NationalityId");
            CreateIndex("dbo.Customers", "CustTypeId");
            CreateIndex("dbo.Customers", "AccountNO", unique: true);
            AddForeignKey("dbo.Customers", "NationalityId", "dbo.Nationalities", "NationalityID", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "identityType", "dbo.UserIdentityTypes", "UserIdentityTypeID", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "CustTypeId", "dbo.CustTypes", "CustTypeID", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "CityId", "dbo.Cities", "CityID", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "BranchId", "dbo.Branches", "BranchID", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "UserID", "dbo.AspNetUsers", "Id");
        }
    }
}
