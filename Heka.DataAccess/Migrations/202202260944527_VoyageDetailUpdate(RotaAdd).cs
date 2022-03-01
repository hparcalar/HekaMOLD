namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoyageDetailUpdateRotaAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoyageDetail", "RotaId", c => c.Int());
            CreateIndex("dbo.VoyageDetail", "RotaId");
            AddForeignKey("dbo.VoyageDetail", "RotaId", "dbo.Rota", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VoyageDetail", "RotaId", "dbo.Rota");
            DropIndex("dbo.VoyageDetail", new[] { "RotaId" });
            DropColumn("dbo.VoyageDetail", "RotaId");
        }
    }
}
