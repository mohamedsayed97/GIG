namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VestingRules", "FundId", c => c.Int(nullable: false));
            CreateIndex("dbo.VestingRules", "FundId");
            AddForeignKey("dbo.VestingRules", "FundId", "dbo.fund", "FundID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VestingRules", "FundId", "dbo.fund");
            DropIndex("dbo.VestingRules", new[] { "FundId" });
            DropColumn("dbo.VestingRules", "FundId");
        }
    }
}
