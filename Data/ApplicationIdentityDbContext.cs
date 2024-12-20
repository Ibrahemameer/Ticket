using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using RhinoTicketingSystem.Models;

namespace RhinoTicketingSystem.Data
{
    public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<FileOperationLog> FileOperationLogs { get; set; }
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {
        }

        public ApplicationIdentityDbContext()
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Roles)
                   .WithMany(r => r.Users)
                   .UsingEntity<IdentityUserRole<string>>();

            this.OnModelBuilding(builder);
        }
    }
}