namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PalletStructure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pallet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PalletNo = c.String(),
                        PalletStatus = c.Int(nullable: false),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plant", t => t.PlantId)
                .Index(t => t.PlantId);
            
            AddColumn("dbo.ItemSerial", "PalletId", c => c.Int());
            CreateIndex("dbo.ItemSerial", "PalletId");
            AddForeignKey("dbo.ItemSerial", "PalletId", "dbo.Pallet", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemSerial", "PalletId", "dbo.Pallet");
            DropForeignKey("dbo.Pallet", "PlantId", "dbo.Plant");
            DropIndex("dbo.Pallet", new[] { "PlantId" });
            DropIndex("dbo.ItemSerial", new[] { "PalletId" });
            DropColumn("dbo.ItemSerial", "PalletId");
            DropTable("dbo.Pallet");
        }
    }
}
