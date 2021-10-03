namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcustomer1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "PostalCode", c => c.Int());
            AlterColumn("dbo.Customers", "tel1", c => c.Int());
            AlterColumn("dbo.Customers", "tel2", c => c.Int());
            AlterColumn("dbo.Customers", "tel3", c => c.Int());
            AlterColumn("dbo.Customers", "Fax1", c => c.Int());
            AlterColumn("dbo.Customers", "Fax2", c => c.Int());
            AlterColumn("dbo.Customers", "Fax3", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "Fax3", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "Fax2", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "Fax1", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "tel3", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "tel2", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "tel1", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "PostalCode", c => c.Int(nullable: false));
        }
    }
}
