using Requirement_Management.DataAccess;
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
        public DbSet<CompanyProvider> CompanyProvider { get; set; }
        public DbSet<SoftwareCategory> SoftwareCategory { get; set; }
        public DbSet<Software> Software { get; set; }
        public DbSet<RequirementType> RequirementType { get; set; }
        public DbSet<Requirement> Requirement { get; set; }
        public DbSet<RequirementDetail> RequirementDetail { get; set; }
        public DbSet<RequirementSoftware> RequirementSoftware { get; set; }
        public DbSet<RequirementFile> RequirementFile { get; set; }
        public DbSet<JobHolder> JobHolders { get; set; }
        public DbSet<ManageRequirement> ManageRequirement { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectSoftware> ProjectSoftware { get; set; }
        public DbSet<ProjectSchedule> ProjectSchedule { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .Map(m =>
            {
                m.ToTable("UserRoles");
                m.MapLeftKey("UserId");
                m.MapRightKey("RoleId");
            });
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }

    }
}