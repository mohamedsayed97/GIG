namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VestingRules", "FundId", "dbo.fund");
            DropIndex("dbo.VestingRules", new[] { "FundId" });
            AlterColumn("dbo.VestingRules", "FundId", c => c.Int());
            CreateIndex("dbo.VestingRules", "FundId");
            AddForeignKey("dbo.VestingRules", "FundId", "dbo.fund", "FundID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VestingRules", "FundId", "dbo.fund");
            DropIndex("dbo.VestingRules", new[] { "FundId" });
            AlterColumn("dbo.VestingRules", "FundId", c => c.Int(nullable: false));
            CreateIndex("dbo.VestingRules", "FundId");
            AddForeignKey("dbo.VestingRules", "FundId", "dbo.fund", "FundID", cascadeDelete: true);
        }
    }
}
