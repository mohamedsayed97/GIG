namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExcelTbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExcelDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        uploadDate = c.DateTime(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                        Screen = c.Byte(nullable: false),
                        Status = c.Byte(nullable: false),
                        File = c.Binary(),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExcelDetails");
        }
    }
}
