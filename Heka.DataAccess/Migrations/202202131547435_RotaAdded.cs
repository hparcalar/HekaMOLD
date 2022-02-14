namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RotaAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rota",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KmHour = c.Int(nullable: false),
                        ProfileImage = c.Binary(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Rota");
        }
    }
}
