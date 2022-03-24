namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadDetailUpdateExplanationAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoadDetail", "Explanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoadDetail", "Explanation");
        }
    }
}
