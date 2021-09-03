namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientCompany",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        Contact = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requirement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                        EntryDate = c.DateTime(nullable: false),
                        CompanyId = c.Int(),
                        ReqTypeId = c.Int(),
                        ReqProviderId = c.Int(),
                        Status = c.Int(nullable: false),
                        StarMarked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientCompany", t => t.CompanyId)
                .ForeignKey("dbo.RequirementProvider", t => t.ReqProviderId)
                .ForeignKey("dbo.RequirementType", t => t.ReqTypeId)
                .Index(t => t.CompanyId)
                .Index(t => t.ReqTypeId)
                .Index(t => t.ReqProviderId);
            
            CreateTable(
                "dbo.RequirementProvider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Contact = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RequirementType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Software",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Software_CategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareCategory", t => t.Software_CategoryId)
                .Index(t => t.Software_CategoryId);
            
            CreateTable(
                "dbo.SoftwareCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Software", "Software_CategoryId", "dbo.SoftwareCategory");
            DropForeignKey("dbo.Requirement", "ReqTypeId", "dbo.RequirementType");
            DropForeignKey("dbo.Requirement", "ReqProviderId", "dbo.RequirementProvider");
            DropForeignKey("dbo.Requirement", "CompanyId", "dbo.ClientCompany");
            DropIndex("dbo.Software", new[] { "Software_CategoryId" });
            DropIndex("dbo.Requirement", new[] { "ReqProviderId" });
            DropIndex("dbo.Requirement", new[] { "ReqTypeId" });
            DropIndex("dbo.Requirement", new[] { "CompanyId" });
            DropTable("dbo.SoftwareCategory");
            DropTable("dbo.Software");
            DropTable("dbo.RequirementType");
            DropTable("dbo.RequirementProvider");
            DropTable("dbo.Requirement");
            DropTable("dbo.ClientCompany");
        }
    }
}
