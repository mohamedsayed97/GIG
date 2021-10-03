namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSubAndTrans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscription", "PolicNo", c => c.String(nullable: false));
            AddColumn("dbo.Subscription", "Policy_Id", c => c.Int());
            AddColumn("dbo.Trans", "PolicNo", c => c.String(nullable: false));
            CreateIndex("dbo.Subscription", "Policy_Id");
            AddForeignKey("dbo.Subscription", "Policy_Id", "dbo.Policies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscription", "Policy_Id", "dbo.Policies");
            DropIndex("dbo.Subscription", new[] { "Policy_Id" });
            DropColumn("dbo.Trans", "PolicNo");
            DropColumn("dbo.Subscription", "Policy_Id");
            DropColumn("dbo.Subscription", "PolicNo");
        }
    }
}
