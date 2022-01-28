﻿namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemLoadDetailUpdateLoadStatusdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemLoadDetail", "LoadStatus", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemLoadDetail", "LoadStatus");
        }
    }
}
