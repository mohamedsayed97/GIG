namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscriptions", "UserID", c => c.String(maxLength: 128));
            AddColumn("dbo.Subscriptions", "SysDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Subscriptions", "ICPrice_ICPriceID", c => c.Int());
            AlterColumn("dbo.Subscriptions", "cust_id", c => c.Int(nullable: false));
            AlterColumn("dbo.Subscriptions", "delreason", c => c.Int(nullable: false));
            CreateIndex("dbo.Subscriptions", "branch_id");
            CreateIndex("dbo.Subscriptions", "cust_id");
            CreateIndex("dbo.Subscriptions", "fund_id");
            CreateIndex("dbo.Subscriptions", "UserID");
            CreateIndex("dbo.Subscriptions", "ICPrice_ICPriceID");
            AddForeignKey("dbo.Subscriptions", "UserID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Subscriptions", "branch_id", "dbo.Branches", "BranchID");
            AddForeignKey("dbo.Subscriptions", "cust_id", "dbo.Customers", "CustomerID", cascadeDelete: true);
            AddForeignKey("dbo.Subscriptions", "fund_id", "dbo.Funds", "FundID");
            AddForeignKey("dbo.Subscriptions", "ICPrice_ICPriceID", "dbo.ICPrices", "ICPriceID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "ICPrice_ICPriceID", "dbo.ICPrices");
            DropForeignKey("dbo.Subscriptions", "fund_id", "dbo.Funds");
            DropForeignKey("dbo.Subscriptions", "cust_id", "dbo.Customers");
            DropForeignKey("dbo.Subscriptions", "branch_id", "dbo.Branches");
            DropForeignKey("dbo.Subscriptions", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Subscriptions", new[] { "ICPrice_ICPriceID" });
            DropIndex("dbo.Subscriptions", new[] { "UserID" });
            DropIndex("dbo.Subscriptions", new[] { "fund_id" });
            DropIndex("dbo.Subscriptions", new[] { "cust_id" });
            DropIndex("dbo.Subscriptions", new[] { "branch_id" });
            AlterColumn("dbo.Subscriptions", "delreason", c => c.Int());
            AlterColumn("dbo.Subscriptions", "cust_id", c => c.String(maxLength: 30));
            DropColumn("dbo.Subscriptions", "ICPrice_ICPriceID");
            DropColumn("dbo.Subscriptions", "SysDate");
            DropColumn("dbo.Subscriptions", "UserID");
        }
    }
}
