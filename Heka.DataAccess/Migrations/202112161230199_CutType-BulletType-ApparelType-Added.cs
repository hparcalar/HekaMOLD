namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CutTypeBulletTypeApparelTypeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "ItemCutType", c => c.Int());
            AddColumn("dbo.Item", "ItemApparelType", c => c.Int());
            AddColumn("dbo.Item", "ItemBulletType", c => c.Int());
            DropColumn("dbo.Item", "Cut");
            DropColumn("dbo.Item", "Apparel");
            DropColumn("dbo.Item", "Bullet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Item", "Bullet", c => c.String());
            AddColumn("dbo.Item", "Apparel", c => c.String());
            AddColumn("dbo.Item", "Cut", c => c.String());
            DropColumn("dbo.Item", "ItemBulletType");
            DropColumn("dbo.Item", "ItemApparelType");
            DropColumn("dbo.Item", "ItemCutType");
        }
    }
}
