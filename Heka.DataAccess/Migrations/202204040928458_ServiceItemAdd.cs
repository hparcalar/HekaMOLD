namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceItemAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceItemCode = c.String(),
                        ServiceItemName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ServiceItem");
        }
    }
}
