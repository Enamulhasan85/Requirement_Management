using Requirement_Management.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Requirement_Management.Models
{
    public class RequirementManagementContext : DbContext
    {

        public RequirementManagementContext() : base("RequirementManagementContext")
        {
        }

        public DbSet<ClientCompany> ClientCompany { get; set; }
        public DbSet<RequirementProvider> RequirementProvider { get; set; }
        public DbSet<SoftwareCategory> SoftwareCategory { get; set; }
        public DbSet<Software> Software { get; set; }
        public DbSet<RequirementType> RequirementType { get; set; }
        public DbSet<Requirement> Requirement { get; set; }
        public DbSet<RequirementDetail> RequirementDetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        
    }
}