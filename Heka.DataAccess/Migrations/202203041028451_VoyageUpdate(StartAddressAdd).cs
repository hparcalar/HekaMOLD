namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageUpdateStartAddressAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Voyage", "StartAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Voyage", "StartAddress");
        }
    }
}
