namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Nationalities", "ShortName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Nationalities", "ShortName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
