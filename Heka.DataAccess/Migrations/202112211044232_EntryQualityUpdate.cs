namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntryQualityUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EntryQualityDataDetail", "CheckResult", c => c.Boolean());
            AddColumn("dbo.EntryQualityDataDetail", "NumericResult", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.EntryQualityDataDetail", "IsOk", c => c.Boolean());
            AddColumn("dbo.EntryQualityPlanDetail", "CheckType", c => c.Int());
            AddColumn("dbo.EntryQualityPlanDetail", "ToleranceMin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.EntryQualityPlanDetail", "ToleranceMax", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EntryQualityPlanDetail", "ToleranceMax");
            DropColumn("dbo.EntryQualityPlanDetail", "ToleranceMin");
            DropColumn("dbo.EntryQualityPlanDetail", "CheckType");
            DropColumn("dbo.EntryQualityDataDetail", "IsOk");
            DropColumn("dbo.EntryQualityDataDetail", "NumericResult");
            DropColumn("dbo.EntryQualityDataDetail", "CheckResult");
        }
    }
}
