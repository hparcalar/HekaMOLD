namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SerialFaultTypeAdded2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SerialFaultType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaultCode = c.String(),
                        FaultName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SerialFaultType");
        }
    }
}
