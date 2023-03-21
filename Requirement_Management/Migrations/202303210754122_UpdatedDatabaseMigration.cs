namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedDatabaseMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requirement", "ReqProviderId", "dbo.RequirementProvider");
            DropIndex("dbo.Requirement", new[] { "ReqProviderId" });
            CreateTable(
                "dbo.CompanyProvider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(),
                        ReqProviderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientCompany", t => t.CompanyId)
                .ForeignKey("dbo.RequirementProvider", t => t.ReqProviderId)
                .Index(t => t.CompanyId)
                .Index(t => t.ReqProviderId);
            
            CreateTable(
                "dbo.JobHolder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Contact = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        Designation = c.String(),
                        JoiningDate = c.DateTime(),
                        HourlyCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ManageRequirement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JobHolderId = c.Int(),
                        RequirementDetailId = c.Int(nullable: false),
                        CurrStatus = c.Int(nullable: false),
                        TargetStatus = c.Int(nullable: false),
                        AssignDate = c.DateTime(nullable: false),
                        DeadLine = c.DateTime(nullable: false),
                        TargetWorkhours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourlyCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkhoursConsumed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompType = c.Int(),
                        CompDate = c.DateTime(),
                        Note = c.String(),
                        DevNote = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JobHolder", t => t.JobHolderId)
                .ForeignKey("dbo.RequirementDetail", t => t.RequirementDetailId, cascadeDelete: true)
                .Index(t => t.JobHolderId)
                .Index(t => t.RequirementDetailId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientCompany", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ProjectSchedule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(),
                        ProjectMode = c.Int(nullable: false),
                        TargetCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        TargetDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.ProjectSoftware",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(),
                        SoftwareId = c.Int(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .ForeignKey("dbo.Software", t => t.SoftwareId)
                .Index(t => t.ProjectId)
                .Index(t => t.SoftwareId);
            
            AddColumn("dbo.RequirementDetail", "ReqProviderId", c => c.Int());
            AddColumn("dbo.RequirementDetail", "ProjectId", c => c.Int());
            AddColumn("dbo.RequirementDetail", "ProjectScheduleId", c => c.Int());
            AddColumn("dbo.RequirementDetail", "ReqMode", c => c.Int(nullable: false));
            AddColumn("dbo.RequirementDetail", "Priority", c => c.Int(nullable: false));
            CreateIndex("dbo.RequirementDetail", "ReqProviderId");
            CreateIndex("dbo.RequirementDetail", "ProjectId");
            CreateIndex("dbo.RequirementDetail", "ProjectScheduleId");
            AddForeignKey("dbo.RequirementDetail", "ProjectId", "dbo.Project", "Id");
            AddForeignKey("dbo.RequirementDetail", "ProjectScheduleId", "dbo.ProjectSchedule", "Id");
            AddForeignKey("dbo.RequirementDetail", "ReqProviderId", "dbo.RequirementProvider", "Id");
            DropColumn("dbo.Requirement", "ReqProviderId");
            DropColumn("dbo.RequirementDetail", "Workdays");
            DropColumn("dbo.RequirementDetail", "StarMarked");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RequirementDetail", "StarMarked", c => c.Boolean(nullable: false));
            AddColumn("dbo.RequirementDetail", "Workdays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Requirement", "ReqProviderId", c => c.Int());
            DropForeignKey("dbo.ProjectSoftware", "SoftwareId", "dbo.Software");
            DropForeignKey("dbo.ProjectSoftware", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.ManageRequirement", "RequirementDetailId", "dbo.RequirementDetail");
            DropForeignKey("dbo.RequirementDetail", "ReqProviderId", "dbo.RequirementProvider");
            DropForeignKey("dbo.RequirementDetail", "ProjectScheduleId", "dbo.ProjectSchedule");
            DropForeignKey("dbo.ProjectSchedule", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.RequirementDetail", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "CompanyId", "dbo.ClientCompany");
            DropForeignKey("dbo.ManageRequirement", "JobHolderId", "dbo.JobHolder");
            DropForeignKey("dbo.CompanyProvider", "ReqProviderId", "dbo.RequirementProvider");
            DropForeignKey("dbo.CompanyProvider", "CompanyId", "dbo.ClientCompany");
            DropIndex("dbo.ProjectSoftware", new[] { "SoftwareId" });
            DropIndex("dbo.ProjectSoftware", new[] { "ProjectId" });
            DropIndex("dbo.ProjectSchedule", new[] { "ProjectId" });
            DropIndex("dbo.Project", new[] { "CompanyId" });
            DropIndex("dbo.RequirementDetail", new[] { "ProjectScheduleId" });
            DropIndex("dbo.RequirementDetail", new[] { "ProjectId" });
            DropIndex("dbo.RequirementDetail", new[] { "ReqProviderId" });
            DropIndex("dbo.ManageRequirement", new[] { "RequirementDetailId" });
            DropIndex("dbo.ManageRequirement", new[] { "JobHolderId" });
            DropIndex("dbo.CompanyProvider", new[] { "ReqProviderId" });
            DropIndex("dbo.CompanyProvider", new[] { "CompanyId" });
            DropColumn("dbo.RequirementDetail", "Priority");
            DropColumn("dbo.RequirementDetail", "ReqMode");
            DropColumn("dbo.RequirementDetail", "ProjectScheduleId");
            DropColumn("dbo.RequirementDetail", "ProjectId");
            DropColumn("dbo.RequirementDetail", "ReqProviderId");
            DropTable("dbo.ProjectSoftware");
            DropTable("dbo.ProjectSchedule");
            DropTable("dbo.Project");
            DropTable("dbo.ManageRequirement");
            DropTable("dbo.JobHolder");
            DropTable("dbo.CompanyProvider");
            CreateIndex("dbo.Requirement", "ReqProviderId");
            AddForeignKey("dbo.Requirement", "ReqProviderId", "dbo.RequirementProvider", "Id");
        }
    }
}
