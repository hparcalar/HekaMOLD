namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateLoadCustomsChangeExitCustoms : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Voyage", name: "DischargeCustomsId", newName: "entryCustomsId");
            RenameColumn(table: "dbo.Voyage", name: "LoadCustomsId", newName: "ExitCustomsId");
            RenameIndex(table: "dbo.Voyage", name: "IX_LoadCustomsId", newName: "IX_ExitCustomsId");
            RenameIndex(table: "dbo.Voyage", name: "IX_DischargeCustomsId", newName: "IX_entryCustomsId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Voyage", name: "IX_entryCustomsId", newName: "IX_DischargeCustomsId");
            RenameIndex(table: "dbo.Voyage", name: "IX_ExitCustomsId", newName: "IX_LoadCustomsId");
            RenameColumn(table: "dbo.Voyage", name: "ExitCustomsId", newName: "LoadCustomsId");
            RenameColumn(table: "dbo.Voyage", name: "entryCustomsId", newName: "DischargeCustomsId");
        }
    }
}
