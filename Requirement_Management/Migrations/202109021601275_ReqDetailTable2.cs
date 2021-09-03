namespace Requirement_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReqDetailTable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequirementDetail", "Requirement", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequirementDetail", "Requirement");
        }
    }
}
