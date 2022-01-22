namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customsUpdate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomsDoor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomsDoorCode = c.String(),
                        CustomsDoorName = c.String(),
                        PlantId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomsDoor");
        }
    }
}
