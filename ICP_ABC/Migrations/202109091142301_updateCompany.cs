namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCompany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        DeletFlag = c.Int(nullable: false),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Policies", "CompanyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Policies", "CompanyId");
            AddForeignKey("dbo.Policies", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Policies", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Policies", new[] { "CompanyId" });
            DropColumn("dbo.Policies", "CompanyId");
            DropTable("dbo.Companies");
        }
    }
}
