namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRedemption : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Redemption", "branch_id", "dbo.Branch");
            DropForeignKey("dbo.Redemption", "fund_id", "dbo.fund");
            DropIndex("dbo.Redemption", new[] { "branch_id" });
            DropIndex("dbo.Redemption", new[] { "fund_id" });
            AddColumn("dbo.Redemption", "GTF_Flag", c => c.Short());
            AlterColumn("dbo.Redemption", "branch_id", c => c.Int());
            AlterColumn("dbo.Redemption", "fund_id", c => c.Int());
            AlterColumn("dbo.Redemption", "appliction_no", c => c.Int());
            AlterColumn("dbo.Redemption", "amount_3", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Redemption", "auth", c => c.Short());
            AlterColumn("dbo.Redemption", "flag_tr", c => c.Short());
            AlterColumn("dbo.Redemption", "pay_method", c => c.Short());
            AlterColumn("dbo.Redemption", "other_fees", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Redemption", "unauth", c => c.Int());
            AlterColumn("dbo.Redemption", "Flag", c => c.Int());
            AlterColumn("dbo.Redemption", "Chk", c => c.Boolean());
            CreateIndex("dbo.Redemption", "branch_id");
            CreateIndex("dbo.Redemption", "fund_id");
            AddForeignKey("dbo.Redemption", "branch_id", "dbo.Branch", "BranchID");
            AddForeignKey("dbo.Redemption", "fund_id", "dbo.fund", "FundID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Redemption", "fund_id", "dbo.fund");
            DropForeignKey("dbo.Redemption", "branch_id", "dbo.Branch");
            DropIndex("dbo.Redemption", new[] { "fund_id" });
            DropIndex("dbo.Redemption", new[] { "branch_id" });
            AlterColumn("dbo.Redemption", "Chk", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Redemption", "Flag", c => c.Int(nullable: false));
            AlterColumn("dbo.Redemption", "unauth", c => c.Int(nullable: false));
            AlterColumn("dbo.Redemption", "other_fees", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Redemption", "pay_method", c => c.Short(nullable: false));
            AlterColumn("dbo.Redemption", "flag_tr", c => c.Short(nullable: false));
            AlterColumn("dbo.Redemption", "auth", c => c.Short(nullable: false));
            AlterColumn("dbo.Redemption", "amount_3", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Redemption", "appliction_no", c => c.Int(nullable: false));
            AlterColumn("dbo.Redemption", "fund_id", c => c.Int(nullable: false));
            AlterColumn("dbo.Redemption", "branch_id", c => c.Int(nullable: false));
            DropColumn("dbo.Redemption", "GTF_Flag");
            CreateIndex("dbo.Redemption", "fund_id");
            CreateIndex("dbo.Redemption", "branch_id");
            AddForeignKey("dbo.Redemption", "fund_id", "dbo.fund", "FundID", cascadeDelete: true);
            AddForeignKey("dbo.Redemption", "branch_id", "dbo.Branch", "BranchID", cascadeDelete: true);
        }
    }
}
