namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                        Date = c.DateTime(nullable: false),
                        EntryDate = c.DateTime(nullable: false),
                        CompanyId = c.Int(),
                        ReqProviderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientCompany", t => t.CompanyId)
                .ForeignKey("dbo.RequirementProvider", t => t.ReqProviderId)
                .Index(t => t.CompanyId)
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
                "dbo.RequirementDetail",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Requirement = c.String(),
                        Description = c.String(),
                        Workdays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SoftCategoryId = c.Int(),
                        ReqTypeId = c.Int(),
                        ReqId = c.Int(),
                        Status = c.Int(nullable: false),
                        StarMarked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requirement", t => t.ReqId)
                .ForeignKey("dbo.RequirementType", t => t.ReqTypeId)
                .ForeignKey("dbo.SoftwareCategory", t => t.SoftCategoryId)
                .Index(t => t.SoftCategoryId)
                .Index(t => t.ReqTypeId)
                .Index(t => t.ReqId);
            
            CreateTable(
                "dbo.RequirementType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SoftwareCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequirementSoftware", "SoftwareId", "dbo.Software");
            DropForeignKey("dbo.Software", "Software_CategoryId", "dbo.SoftwareCategory");
            DropForeignKey("dbo.RequirementSoftware", "RequirementDetailId", "dbo.RequirementDetail");
            DropForeignKey("dbo.RequirementDetail", "SoftCategoryId", "dbo.SoftwareCategory");
            DropForeignKey("dbo.RequirementDetail", "ReqTypeId", "dbo.RequirementType");
            DropForeignKey("dbo.RequirementDetail", "ReqId", "dbo.Requirement");
            DropForeignKey("dbo.Requirement", "ReqProviderId", "dbo.RequirementProvider");
            DropForeignKey("dbo.Requirement", "CompanyId", "dbo.ClientCompany");
            DropIndex("dbo.Software", new[] { "Software_CategoryId" });
            DropIndex("dbo.RequirementSoftware", new[] { "RequirementDetailId" });
            DropIndex("dbo.RequirementSoftware", new[] { "SoftwareId" });
            DropIndex("dbo.RequirementDetail", new[] { "ReqId" });
            DropIndex("dbo.RequirementDetail", new[] { "ReqTypeId" });
            DropIndex("dbo.RequirementDetail", new[] { "SoftCategoryId" });
            DropIndex("dbo.Requirement", new[] { "ReqProviderId" });
            DropIndex("dbo.Requirement", new[] { "CompanyId" });
            DropTable("dbo.Software");
            DropTable("dbo.RequirementSoftware");
            DropTable("dbo.SoftwareCategory");
            DropTable("dbo.RequirementType");
            DropTable("dbo.RequirementDetail");
            DropTable("dbo.RequirementProvider");
            DropTable("dbo.Requirement");
            DropTable("dbo.ClientCompany");
        }
    }
}
