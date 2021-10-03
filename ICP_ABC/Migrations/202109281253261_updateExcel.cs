namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExcel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExcelDetails", "FileContent", c => c.Binary());
            AddColumn("dbo.ExcelDetails", "ContentType", c => c.String());
            AddColumn("dbo.ExcelDetails", "FileSize", c => c.Int(nullable: false));
            AddColumn("dbo.ExcelDetails", "FileExtension", c => c.String());
            DropColumn("dbo.ExcelDetails", "createdDate");
            DropColumn("dbo.ExcelDetails", "modifiedDate");
            DropColumn("dbo.ExcelDetails", "File");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExcelDetails", "File", c => c.Binary());
            AddColumn("dbo.ExcelDetails", "modifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ExcelDetails", "createdDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ExcelDetails", "FileExtension");
            DropColumn("dbo.ExcelDetails", "FileSize");
            DropColumn("dbo.ExcelDetails", "ContentType");
            DropColumn("dbo.ExcelDetails", "FileContent");
        }
    }
}
