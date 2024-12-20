using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RhinoTicketingSystem.Models.db_a79800_ticket;
using RhinoTicketingSystem.Models.DbA79800Ticket;
using System.Reflection.Emit;


namespace RhinoTicketingSystem.Data
{
    public partial class db_a79800_ticketContext : DbContext
    {
        public db_a79800_ticketContext()
        {
        }

        public db_a79800_ticketContext(DbContextOptions<db_a79800_ticketContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ignore existing tables
            builder.Ignore<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus>();
            builder.Ignore<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType>();

            // Map existing tables
            builder.Entity<OneDriveArchivingHeader>()
                .ToTable("OneDriveArchivingHeaders");
            builder.Entity<OneDriveArchivingDetail>()
                .ToTable("OneDriveArchivingDetails");

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.Task>()
              .HasOne(i => i.TblEngineer)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.EngineerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.Task>()
              .HasOne(i => i.TaskStatus)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.StatusId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.Task>()
              .HasOne(i => i.TaskType)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.TypeId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount>()
              .HasOne(i => i.TblChartOfAccount1)
              .WithMany(i => i.TblChartOfAccounts1)
              .HasForeignKey(i => i.ParentAccount)
              .HasPrincipalKey(i => i.ChartId);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket>()
              .HasOne(i => i.TblEngineer)
              .WithMany(i => i.TblReassignTickets)
              .HasForeignKey(i => i.EngineerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket>()
              .HasOne(i => i.TblStatus)
              .WithMany(i => i.TblReassignTickets)
              .HasForeignKey(i => i.StatusId)
              .HasPrincipalKey(i => i.StatusId);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket>()
              .HasOne(i => i.TblTicket)
              .WithMany(i => i.TblReassignTickets)
              .HasForeignKey(i => i.TicketId)
              .HasPrincipalKey(i => i.TicketId);
            builder.Entity<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment>()
              .HasOne(i => i.tblTickets)
              .WithMany(i => i.TblTicketattachments)
              .HasForeignKey(i => i.TicketId)
              .HasPrincipalKey(i => i.TicketId);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>()
              .HasOne(i => i.TblCategory)
              .WithMany(i => i.TblTickets)
              .HasForeignKey(i => i.CategoryId)
              .HasPrincipalKey(i => i.CategoryId);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>()
              .HasOne(i => i.TblEmployee)
              .WithMany(i => i.TblTickets)
              .HasForeignKey(i => i.EmployeeId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket>()
              .HasOne(i => i.TblEngineer)
              .WithMany(i => i.TblTickets)
              .HasForeignKey(i => i.EngineerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail>()
              .HasOne(i => i.TblEngineer)
              .WithMany(i => i.TblTaskDetails)
              .HasForeignKey(i => i.EngineerId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<OneDriveArchivingDetail>()
              .HasOne(d => d.ArchiveHeader)
              .WithMany(h => h.ArchiveDetails)
              .HasForeignKey(d => d.ArchiveId);


            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail>()
              .HasOne(i => i.Task)
              .WithMany(i => i.TblTaskDetails)
              .HasForeignKey(i => i.TaskId)
              .HasPrincipalKey(i => i.Id);


            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.Task>()
              .Property(p => p.CreatedDate)
              .HasColumnType("datetime");

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.Task>()
              .Property(p => p.DueDate)
              .HasColumnType("datetime");

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType>()
              .Property(p => p.CreatedDate)
              .HasColumnType("datetime");

            builder.Entity<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail>()
              .Property(p => p.ActionDate)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);



        }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.Task> Tasks { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TaskStatus> TaskStatuses { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TaskType> TaskTypes { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblCategory> TblCategories { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblChartOfAccount> TblChartOfAccounts { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblEmployee> TblEmployees { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblEngineer> TblEngineers { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblReassignTicket> TblReassignTickets { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblStatus> TblStatuses { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblTicket> TblTickets { get; set; }

        public DbSet<RhinoTicketingSystem.Models.db_a79800_ticket.TblTaskDetail> TblTaskDetails { get; set; }
        public DbSet<RhinoTicketingSystem.Models.DbA79800Ticket.TblTicketattachment> TblTicketattachments { get; set; }


        public DbSet<OneDriveArchivingHeader> OneDriveArchivingHeaders { get; set; }
        public DbSet<OneDriveArchivingDetail> OneDriveArchivingDetails { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}