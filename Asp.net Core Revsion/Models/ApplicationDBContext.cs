using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp.net_Core_Revsion.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Department>()
                .HasData(
                            new Department() { Id = 1, Name = "IT", Location = "Cairo" },
                            new Department() { Id = 2, Name = "CS", Location = "Alex" },
                            new Department() { Id = 3, Name = "IS", Location = "Giza" }
                        );
            base.OnModelCreating(modelBuilder);
        }
    }
}