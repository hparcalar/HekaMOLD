namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadRevize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "ReadinessDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "DeliveryFromCustomerDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "IntendedArrivalDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "ArrivalCustoms", c => c.String());
            AddColumn("dbo.ItemLoad", "CustomsExplanation", c => c.String());
            AddColumn("dbo.ItemLoad", "T1T2No", c => c.String());
            AddColumn("dbo.ItemLoad", "TClosingDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "CrmDeliveryDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "ItemPrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ItemLoad", "TrailerType", c => c.Int());
            AddColumn("dbo.ItemLoad", "HasItemInsurance", c => c.Boolean());
            AddColumn("dbo.ItemLoad", "ItemInsuranceDraftNo", c => c.String());
            AddColumn("dbo.ItemLoad", "HasItemDangerous", c => c.Boolean());
            AddColumn("dbo.ItemLoad", "CmrCustomerDeliveryDate", c => c.DateTime());
            AddColumn("dbo.ItemLoad", "BringingToWarehousePlate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "BringingToWarehousePlate");
            DropColumn("dbo.ItemLoad", "CmrCustomerDeliveryDate");
            DropColumn("dbo.ItemLoad", "HasItemDangerous");
            DropColumn("dbo.ItemLoad", "ItemInsuranceDraftNo");
            DropColumn("dbo.ItemLoad", "HasItemInsurance");
            DropColumn("dbo.ItemLoad", "TrailerType");
            DropColumn("dbo.ItemLoad", "ItemPrice");
            DropColumn("dbo.ItemLoad", "CrmDeliveryDate");
            DropColumn("dbo.ItemLoad", "TClosingDate");
            DropColumn("dbo.ItemLoad", "T1T2No");
            DropColumn("dbo.ItemLoad", "CustomsExplanation");
            DropColumn("dbo.ItemLoad", "ArrivalCustoms");
            DropColumn("dbo.ItemLoad", "IntendedArrivalDate");
            DropColumn("dbo.ItemLoad", "DeliveryFromCustomerDate");
            DropColumn("dbo.ItemLoad", "ReadinessDate");
        }
    }
}
