namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeavingDraftAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeavingDraft",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WeavingDraftCode = c.String(),
                        MachineBreedId = c.Int(),
                        NumberOfFramaes = c.String(),
                        Report = c.String(),
                        PlatinumNumber = c.Int(),
                        JakarReport = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MachineBreed", t => t.MachineBreedId)
                .Index(t => t.MachineBreedId);
            
            AddColumn("dbo.Item", "WeavingDraftId", c => c.Int());
            CreateIndex("dbo.Item", "WeavingDraftId");
            AddForeignKey("dbo.Item", "WeavingDraftId", "dbo.WeavingDraft", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeavingDraft", "MachineBreedId", "dbo.MachineBreed");
            DropForeignKey("dbo.Item", "WeavingDraftId", "dbo.WeavingDraft");
            DropIndex("dbo.WeavingDraft", new[] { "MachineBreedId" });
            DropIndex("dbo.Item", new[] { "WeavingDraftId" });
            DropColumn("dbo.Item", "WeavingDraftId");
            DropTable("dbo.WeavingDraft");
        }
    }
}
