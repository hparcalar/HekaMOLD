namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdatecreatedUserIdUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ItemOrder", "CreatedUserId");
            RenameColumn(table: "dbo.ItemOrder", name: "CreatUserId", newName: "CreatedUserId");
            RenameIndex(table: "dbo.ItemOrder", name: "IX_CreatUserId", newName: "IX_CreatedUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemOrder", name: "IX_CreatedUserId", newName: "IX_CreatUserId");
            RenameColumn(table: "dbo.ItemOrder", name: "CreatedUserId", newName: "CreatUserId");
            AddColumn("dbo.ItemOrder", "CreatedUserId", c => c.Int());
        }
    }
}
