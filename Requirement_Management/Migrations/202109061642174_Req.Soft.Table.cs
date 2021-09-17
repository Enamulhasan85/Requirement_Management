namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReqSoftTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequirementSoftware",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SoftwareId = c.Int(),
                        RequirementDetailId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RequirementDetail", t => t.RequirementDetailId)
                .ForeignKey("dbo.Software", t => t.SoftwareId)
                .Index(t => t.SoftwareId)
                .Index(t => t.RequirementDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequirementSoftware", "SoftwareId", "dbo.Software");
            DropForeignKey("dbo.RequirementSoftware", "RequirementDetailId", "dbo.RequirementDetail");
            DropIndex("dbo.RequirementSoftware", new[] { "RequirementDetailId" });
            DropIndex("dbo.RequirementSoftware", new[] { "SoftwareId" });
            DropTable("dbo.RequirementSoftware");
        }
    }
}
