namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadUpdateDeclarationX1NoAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoad", "DeclarationX1No", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoad", "DeclarationX1No");
        }
    }
}
