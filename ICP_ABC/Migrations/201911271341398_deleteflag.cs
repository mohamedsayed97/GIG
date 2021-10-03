namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteflag : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.GroupRights", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.FundRights", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.Funds", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.Currencies", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.Sponsors", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.Cities", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.FundTimes", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.ICPrices", "DeletFlag", c => c.Int(nullable: false));
            AlterColumn("dbo.UserSecurities", "DeletFlag", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserSecurities", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ICPrices", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FundTimes", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Customers", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Cities", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Sponsors", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Currencies", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Funds", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FundRights", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.GroupRights", "DeletFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Branches", "DeletFlag", c => c.Boolean(nullable: false));
        }
    }
}
