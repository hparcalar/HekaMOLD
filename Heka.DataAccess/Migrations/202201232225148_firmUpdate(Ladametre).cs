namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firmUpdateLadametre : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Firm", "LadametrePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Firm", "MeterCupPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Firm", "WeightPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Firm", "ForexTypeId", c => c.Int());
            CreateIndex("dbo.Firm", "ForexTypeId");
            AddForeignKey("dbo.Firm", "ForexTypeId", "dbo.ForexType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Firm", "ForexTypeId", "dbo.ForexType");
            DropIndex("dbo.Firm", new[] { "ForexTypeId" });
            DropColumn("dbo.Firm", "ForexTypeId");
            DropColumn("dbo.Firm", "WeightPrice");
            DropColumn("dbo.Firm", "MeterCupPrice");
            DropColumn("dbo.Firm", "LadametrePrice");
        }
    }
}
