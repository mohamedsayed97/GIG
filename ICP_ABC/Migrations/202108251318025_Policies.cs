namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Policies : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Policies",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            PolicyNo = c.String(nullable: false, maxLength: 200),
            //            PolicyHolderName = c.String(nullable: false),
            //            PaymentFrequency = c.Byte(nullable: false),
            //            EffectiveDate = c.DateTime(nullable: false, storeType: "date"),
            //            Status = c.Byte(nullable: false),
            //            CalculationBasis = c.Byte(nullable: false),
            //            BusinessChannel = c.Byte(nullable: false),
            //            Maker = c.String(nullable: false),
            //            Chk = c.Boolean(nullable: false),
            //            Checker = c.String(),
            //            Auth = c.Boolean(nullable: false),
            //            Auther = c.String(),
            //            DeletFlag = c.Int(nullable: false),
            //            SysDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.PolicyNo, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Policies", new[] { "PolicyNo" });
            DropTable("dbo.Policies");
        }
    }
}
