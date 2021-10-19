namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsoftwarecategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequirementDetail", "SoftCategoryId", c => c.Int());
            CreateIndex("dbo.RequirementDetail", "SoftCategoryId");
            AddForeignKey("dbo.RequirementDetail", "SoftCategoryId", "dbo.SoftwareCategory", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequirementDetail", "SoftCategoryId", "dbo.SoftwareCategory");
            DropIndex("dbo.RequirementDetail", new[] { "SoftCategoryId" });
            DropColumn("dbo.RequirementDetail", "SoftCategoryId");
        }
    }
}
