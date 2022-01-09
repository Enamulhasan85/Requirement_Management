namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequirementFileTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequirementFile",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(),
                        ReqId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requirement", t => t.ReqId)
                .Index(t => t.ReqId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequirementFile", "ReqId", "dbo.Requirement");
            DropIndex("dbo.RequirementFile", new[] { "ReqId" });
            DropTable("dbo.RequirementFile");
        }
    }
}
