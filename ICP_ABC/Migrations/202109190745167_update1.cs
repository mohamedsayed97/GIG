namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VestingRules", "FundId", "dbo.fund");
            DropForeignKey("dbo.Subscription", "Policy_Id", "dbo.Policies");
            DropIndex("dbo.Subscription", new[] { "Policy_Id" });
            DropIndex("dbo.VestingRules", new[] { "FundId" });
            RenameColumn(table: "dbo.Subscription", name: "Policy_Id", newName: "PolicyId");
            AddColumn("dbo.Customer", "CompanyId", c => c.Int(nullable: true));
            AddColumn("dbo.Trans", "CustomerID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Subscription", "PolicyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Customer", "CompanyId");
            CreateIndex("dbo.Subscription", "PolicyId");
            CreateIndex("dbo.Trans", "CustomerID");
            AddForeignKey("dbo.Customer", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Trans", "CustomerID", "dbo.Customer", "CustomerID");
            AddForeignKey("dbo.Subscription", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            DropColumn("dbo.Subscription", "PolicNo");
            DropColumn("dbo.VestingRules", "FundId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VestingRules", "FundId", c => c.Int(nullable: false));
            AddColumn("dbo.Subscription", "PolicNo", c => c.String(nullable: false));
            DropForeignKey("dbo.Subscription", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Trans", "CustomerID", "dbo.Customer");
            DropForeignKey("dbo.Customer", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Trans", new[] { "CustomerID" });
            DropIndex("dbo.Subscription", new[] { "PolicyId" });
            DropIndex("dbo.Customer", new[] { "CompanyId" });
            AlterColumn("dbo.Subscription", "PolicyId", c => c.Int());
            DropColumn("dbo.Trans", "CustomerID");
            DropColumn("dbo.Customer", "CompanyId");
            RenameColumn(table: "dbo.Subscription", name: "PolicyId", newName: "Policy_Id");
            CreateIndex("dbo.VestingRules", "FundId");
            CreateIndex("dbo.Subscription", "Policy_Id");
            AddForeignKey("dbo.Subscription", "Policy_Id", "dbo.Policies", "Id");
            AddForeignKey("dbo.VestingRules", "FundId", "dbo.fund", "FundID", cascadeDelete: true);
        }
    }
}
