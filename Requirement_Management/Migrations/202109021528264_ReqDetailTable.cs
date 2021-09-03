namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReqDetailTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requirement", "ReqTypeId", "dbo.RequirementType");
            DropIndex("dbo.Requirement", new[] { "ReqTypeId" });
            CreateTable(
                "dbo.RequirementDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ReqTypeId = c.Int(),
                        ReqId = c.Int(),
                        Status = c.Int(nullable: false),
                        StarMarked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requirement", t => t.ReqId)
                .ForeignKey("dbo.RequirementType", t => t.ReqTypeId)
                .Index(t => t.ReqTypeId)
                .Index(t => t.ReqId);
            
            DropColumn("dbo.Requirement", "Description");
            DropColumn("dbo.Requirement", "ReqTypeId");
            DropColumn("dbo.Requirement", "Status");
            DropColumn("dbo.Requirement", "StarMarked");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requirement", "StarMarked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Requirement", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Requirement", "ReqTypeId", c => c.Int());
            AddColumn("dbo.Requirement", "Description", c => c.String());
            DropForeignKey("dbo.RequirementDetail", "ReqTypeId", "dbo.RequirementType");
            DropForeignKey("dbo.RequirementDetail", "ReqId", "dbo.Requirement");
            DropIndex("dbo.RequirementDetail", new[] { "ReqId" });
            DropIndex("dbo.RequirementDetail", new[] { "ReqTypeId" });
            DropTable("dbo.RequirementDetail");
            CreateIndex("dbo.Requirement", "ReqTypeId");
            AddForeignKey("dbo.Requirement", "ReqTypeId", "dbo.RequirementType", "Id");
        }
    }
}
