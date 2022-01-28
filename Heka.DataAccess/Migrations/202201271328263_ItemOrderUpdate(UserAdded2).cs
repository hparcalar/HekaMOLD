namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemOrderUpdateUserAdded2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ItemOrder", name: "UserId", newName: "CreatUserId");
            RenameIndex(table: "dbo.ItemOrder", name: "IX_UserId", newName: "IX_CreatUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ItemOrder", name: "IX_CreatUserId", newName: "IX_UserId");
            RenameColumn(table: "dbo.ItemOrder", name: "CreatUserId", newName: "UserId");
        }
    }
}
