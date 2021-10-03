namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCusType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustTypes", "LOP", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustTypes", "LOP");
        }
    }
}
