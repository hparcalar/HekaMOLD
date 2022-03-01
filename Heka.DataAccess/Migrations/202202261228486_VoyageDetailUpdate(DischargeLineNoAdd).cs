namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateDischargeLineNoAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Voyage", "OrderTransactionDirectionType", c => c.Int());
            AddColumn("dbo.VoyageDetail", "DischargeLineNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoyageDetail", "DischargeLineNo");
            DropColumn("dbo.Voyage", "OrderTransactionDirectionType");
        }
    }
}
