namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DeleteFlag", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "DeletFlag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "DeletFlag", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "DeleteFlag");
        }
    }
}
