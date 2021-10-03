namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcustomer : DbMigration
    {
        public override void Up()
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
                        PostalCode = c.Int(nullable: false),
                        CRNumber = c.String(),
                        Sector = c.String(),
                        IssuanceDate = c.DateTime(nullable: false),
                        tel1 = c.Int(nullable: false),
                        tel2 = c.Int(nullable: false),
                        tel3 = c.Int(nullable: false),
                        Fax1 = c.Int(nullable: false),
                        Fax2 = c.Int(nullable: false),
                        Fax3 = c.Int(nullable: false),
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
                .PrimaryKey(t => t.CustomerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: false)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: false)
                .ForeignKey("dbo.CustTypes", t => t.CustTypeId, cascadeDelete: false)
                .ForeignKey("dbo.UserIdentityTypes", t => t.identityType, cascadeDelete: false)
                .ForeignKey("dbo.Nationalities", t => t.NationalityId, cascadeDelete: false)
                .Index(t => t.AccountNO, unique: true)
                .Index(t => t.CustTypeId)
                .Index(t => t.NationalityId)
                .Index(t => t.BranchId)
                .Index(t => t.CityId)
                .Index(t => t.identityType)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "NationalityId", "dbo.Nationalities");
            DropForeignKey("dbo.Customers", "identityType", "dbo.UserIdentityTypes");
            DropForeignKey("dbo.Customers", "CustTypeId", "dbo.CustTypes");
            DropForeignKey("dbo.Customers", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Customers", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.Customers", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Customers", new[] { "UserID" });
            DropIndex("dbo.Customers", new[] { "identityType" });
            DropIndex("dbo.Customers", new[] { "CityId" });
            DropIndex("dbo.Customers", new[] { "BranchId" });
            DropIndex("dbo.Customers", new[] { "NationalityId" });
            DropIndex("dbo.Customers", new[] { "CustTypeId" });
            DropIndex("dbo.Customers", new[] { "AccountNO" });
            DropTable("dbo.Customers");
        }
    }
}
