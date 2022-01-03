namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class machineUpdate1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MachineBreed",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineBreedCode = c.String(),
                        MachineBreedName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Machine", "MachineBreedId", c => c.Int());
            CreateIndex("dbo.Machine", "MachineBreedId");
            AddForeignKey("dbo.Machine", "MachineBreedId", "dbo.MachineBreed", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Machine", "MachineBreedId", "dbo.MachineBreed");
            DropIndex("dbo.Machine", new[] { "MachineBreedId" });
            DropColumn("dbo.Machine", "MachineBreedId");
            DropTable("dbo.MachineBreed");
        }
    }
}
