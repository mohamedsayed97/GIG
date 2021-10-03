namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Redemption", "PolicyId", c => c.Int(nullable: false));
            AddColumn("dbo.Trans", "PolicyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Redemption", "PolicyId");
            CreateIndex("dbo.Trans", "PolicyId");
            AddForeignKey("dbo.Redemption", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Trans", "PolicyId", "dbo.Policies", "Id", cascadeDelete: true);
            DropColumn("dbo.Trans", "PolicNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trans", "PolicNo", c => c.String(nullable: false));
            DropForeignKey("dbo.Trans", "PolicyId", "dbo.Policies");
            DropForeignKey("dbo.Redemption", "PolicyId", "dbo.Policies");
            DropIndex("dbo.Trans", new[] { "PolicyId" });
            DropIndex("dbo.Redemption", new[] { "PolicyId" });
            DropColumn("dbo.Trans", "PolicyId");
            DropColumn("dbo.Redemption", "PolicyId");
        }
    }
}
