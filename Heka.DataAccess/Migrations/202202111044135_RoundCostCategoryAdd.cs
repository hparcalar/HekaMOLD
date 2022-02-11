namespace Heka.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoundCostCategoryAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoundCostCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoundCostCategoryCode = c.String(),
                        RoundCostCategoryName = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedUserId = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoundCostCategory");
        }
    }
}
