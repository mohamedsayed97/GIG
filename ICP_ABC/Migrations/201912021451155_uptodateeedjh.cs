namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uptodateeedjh : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ICPrices", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Subscriptions", "sub_fees", c => c.Decimal(precision: 18, scale: 5));
            AlterColumn("dbo.Subscriptions", "total", c => c.Decimal(nullable: false, precision: 25, scale: 2));
            AlterColumn("dbo.Subscriptions", "NAV", c => c.Decimal(nullable: false, precision: 18, scale: 5));
            AlterColumn("dbo.Subscriptions", "other_fees", c => c.Decimal(precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subscriptions", "other_fees", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Subscriptions", "NAV", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Subscriptions", "total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Subscriptions", "sub_fees", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ICPrices", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
