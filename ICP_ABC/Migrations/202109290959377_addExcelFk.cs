namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExcelFk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Redemption", "ExcelId", c => c.Int());
            AddColumn("dbo.Subscription", "ExcelId", c => c.Int());
            AddColumn("dbo.Trans", "ExcelId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trans", "ExcelId");
            DropColumn("dbo.Subscription", "ExcelId");
            DropColumn("dbo.Redemption", "ExcelId");
        }
    }
}
