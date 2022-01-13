namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleTireDirectionType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VehicleTireDirectionType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleTireDirectionTypeCode = c.String(),
                        VehicleTireDirectionTypeName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.VehicleTire", "VehicleTireDirectionTypeId", c => c.Int());
            CreateIndex("dbo.VehicleTire", "VehicleTireDirectionTypeId");
            AddForeignKey("dbo.VehicleTire", "VehicleTireDirectionTypeId", "dbo.VehicleTireDirectionType", "Id");
            DropColumn("dbo.VehicleTire", "DirectionType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleTire", "DirectionType", c => c.Int());
            DropForeignKey("dbo.VehicleTire", "VehicleTireDirectionTypeId", "dbo.VehicleTireDirectionType");
            DropIndex("dbo.VehicleTire", new[] { "VehicleTireDirectionTypeId" });
            DropColumn("dbo.VehicleTire", "VehicleTireDirectionTypeId");
            DropTable("dbo.VehicleTireDirectionType");
        }
    }
}
