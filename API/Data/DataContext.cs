using API.Entities.Junctions;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PC> PCs { get; set; }
        public DbSet<CPU> CPUs { get; set; }
        public DbSet<MOBO> MOBOs { get; set; }
        public DbSet<RAM> RAMs { get; set; }
        public DbSet<GPU> GPUs { get; set; }
        public DbSet<PSU> PSUs { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<NetworkCard> NetworkCards { get; set; }
        public DbSet<Entities.Monitor> Monitors { get; set; }

        // Junction Tables
        public DbSet<Employee_PC> Employee_PCs { get; set; }
        public DbSet<PC_CPU> PC_CPUs { get; set; }
        public DbSet<PC_MOBO> PC_MOBOs { get; set; }
        public DbSet<PC_RAM> PC_RAMs { get; set; }
        public DbSet<PC_GPU> PC_GPUs { get; set; }
        public DbSet<PC_PSU> PC_PSUs { get; set; }
        public DbSet<PC_Storage> PC_Storages { get; set; }
        public DbSet<PC_NetworkCard> PC_NetworkCards { get; set; }
        public DbSet<PC_Monitor> PC_Monitors { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(r => r.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Search Vector
            builder.Entity<Employee>()
                   .HasIndex(e => new {e.AM, e.Rank, e.Speciality, e.FirstName, e.LastName, e.Unit, e.Office, e.Position, e.Phone, e.Mobile, e.Email})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("greek");
            builder.Entity<Employee>()
                   .HasIndex(e => new {e.AM, e.Rank, e.Speciality, e.FirstName, e.LastName, e.Unit, e.Office, e.Position, e.Phone, e.Mobile, e.Email})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<PC>()
                   .HasIndex(p => new {p.Barcode, p.Brand, p.Model, p.IP, p.ExternalIP})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<CPU>()
                   .HasIndex(c => new {c.Barcode, c.Brand, c.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<MOBO>()
                   .HasIndex(m => new {m.Barcode, m.Brand, m.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<RAM>()
                   .HasIndex(r => new {r.Barcode, r.Brand, r.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<GPU>()
                   .HasIndex(g => new {g.Barcode, g.Brand, g.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<PSU>()
                   .HasIndex(p => new {p.Barcode, p.Brand, p.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<Storage>()
                   .HasIndex(s => new {s.Barcode, s.Brand, s.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<NetworkCard>()
                   .HasIndex(n => new {n.Barcode, n.Brand, n.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");
            builder.Entity<Entities.Monitor>()
                   .HasIndex(m => new {m.Barcode, m.Brand, m.Model})
                   .HasMethod("GIN")
                   .IsTsVectorExpressionIndex("english");

            // Relationship Properties
            // Employee-PCs (Many-To-Many)
            builder.Entity<Employee_PC>(entity =>
            {
                entity.HasKey(employeePc => new { employeePc.EmployeeID, employeePc.PCID });
                entity.HasOne(employeePc => employeePc.Employee)            // Navigation to Employee
                    .WithMany(employee => employee.Employee_PCs)            // An Employee can have many PCs
                    .HasForeignKey(employeePc => employeePc.EmployeeID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a Employee removes the relationship.
                entity.HasOne(employeePc => employeePc.PC)                  // Navigation to PC
                    .WithMany(pc => pc.Employee_PCs)                        // A PC can be assigned to many Employees
                    .HasForeignKey(employeePc => employeePc.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
            });
            // PC_CPU (Many-to-Many)
            builder.Entity<PC_CPU>(entity =>
            {
                entity.HasKey(pcCpu => new { pcCpu.PCID, pcCpu.CPUID });
                entity.HasOne(pcCpu => pcCpu.PC)                            // Navigation to PC
                    .WithMany(pc => pc.PC_CPUs)                             // A PC can have many CPUs
                    .HasForeignKey(pcCpu => pcCpu.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcCpu => pcCpu.CPU)                           // Navigation to CPU
                    .WithMany(cpu => cpu.PC_CPUs)                           // A CPU can be assigned to only one PC
                    .HasForeignKey(pcCpu => pcCpu.CPUID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a CPU removes the relationship.
            });
            // PC_MOBO (Many-to-Many)
            builder.Entity<PC_MOBO>(entity =>
            {
                entity.HasKey(pcMobo => new { pcMobo.PCID, pcMobo.MOBOID });
                entity.HasOne(pcMobo => pcMobo.PC)                          // Navigation to PC
                    .WithMany(pc => pc.PC_MOBOs)                            // A PC can have many MOBOs
                    .HasForeignKey(pcMobo => pcMobo.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcMobo => pcMobo.MOBO)                        // Navigation to MOBO
                    .WithMany(mobo => mobo.PC_MOBOs)                        // A MOBO can be assigned to only one PC
                    .HasForeignKey(pcMobo => pcMobo.MOBOID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a MOBO removes the relationship.
            });
            // PC_RAM (Many-to-Many)
            builder.Entity<PC_RAM>(entity =>
            {
                entity.HasKey(pcRam => new { pcRam.PCID, pcRam.RAMID });
                entity.HasOne(pcRam => pcRam.PC)                            // Navigation to PC
                    .WithMany(pc => pc.PC_RAMs)                             // A PC can have many RAMs
                    .HasForeignKey(pcRam => pcRam.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcRam => pcRam.RAM)                           // Navigation to RAM
                    .WithMany(ram => ram.PC_RAMs)                           // A RAM can be assigned to only one PC
                    .HasForeignKey(pcRam => pcRam.RAMID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a RAM removes the relationship.
            });
            // PC_GPU (Many-to-Many)
            builder.Entity<PC_GPU>(entity =>
            {
                entity.HasKey(pcGpu => new { pcGpu.PCID, pcGpu.GPUID });
                entity.HasOne(pcGpu => pcGpu.PC)                            // Navigation to PC
                    .WithMany(pc => pc.PC_GPUs)                             // A PC can have many GPUs
                    .HasForeignKey(pcGpu => pcGpu.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcGpu => pcGpu.GPU)                           // Navigation to GPU
                    .WithMany(gpu => gpu.PC_GPUs)                           // A GPU can be assigned to only one PC
                    .HasForeignKey(pcGpu => pcGpu.GPUID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a GPU removes the relationship.
            });
            // PC_PSU (Many-to-Many)
            builder.Entity<PC_PSU>(entity =>
            {
                entity.HasKey(pcPsu => new { pcPsu.PCID, pcPsu.PSUID });
                entity.HasOne(pcPsu => pcPsu.PC)                            // Navigation to PC
                    .WithMany(pc => pc.PC_PSUs)                             // A PC can have many MOBOs
                    .HasForeignKey(pcPsu => pcPsu.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcPsu => pcPsu.PSU)                           // Navigation to MOBO
                    .WithMany(psu => psu.PC_PSUs)                           // A MOBO can be assigned to only one PC
                    .HasForeignKey(pcPsu => pcPsu.PSUID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a MOBO removes the relationship.
            });
            // PC_Storage (Many-to-Many)
            builder.Entity<PC_Storage>(entity =>
            {
                entity.HasKey(pcStorage => new { pcStorage.PCID, pcStorage.StorageID });
                entity.HasOne(pcStorage => pcStorage.PC)                    // Navigation to PC
                    .WithMany(pc => pc.PC_Storages)                         // A PC can have many Storages
                    .HasForeignKey(pcStorage => pcStorage.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcStorage => pcStorage.Storage)               // Navigation to Storage
                    .WithMany(storage => storage.PC_Storages)               // A Storage can be assigned to only one PC
                    .HasForeignKey(pcStorage => pcStorage.StorageID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a Storage removes the relationship.
            });
            // PC_NetworkCard (Many-to-Many)
            builder.Entity<PC_NetworkCard>(entity =>
            {
                entity.HasKey(pcNetworkcard => new { pcNetworkcard.PCID, pcNetworkcard.NetworkCardID });
                entity.HasOne(pcNetworkcard => pcNetworkcard.PC)            // Navigation to PC
                    .WithMany(pc => pc.PC_NetworkCards)                     // A PC can have many MOBOs
                    .HasForeignKey(pcNetworkcard => pcNetworkcard.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcNetworkcard => pcNetworkcard.NetworkCard)   // Navigation to MOBO
                    .WithMany(networkcard => networkcard.PC_NetworkCards)   // A MOBO can be assigned to only one PC
                    .HasForeignKey(pcNetworkcard => pcNetworkcard.NetworkCardID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a MOBO removes the relationship.
            });
            // PC_Monitor (Many-to-Many)
            builder.Entity<PC_Monitor>(entity =>
            {
                entity.HasKey(pcMonitor => new { pcMonitor.PCID, pcMonitor.MonitorID });
                entity.HasOne(pcMonitor => pcMonitor.PC)                    // Navigation to PC
                    .WithMany(pc => pc.PC_Monitors)                         // A PC can have many MOBOs
                    .HasForeignKey(pcMonitor => pcMonitor.PCID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a PC removes the relationship.
                entity.HasOne(pcMonitor => pcMonitor.Monitor)               // Navigation to MOBO
                    .WithMany(monitor => monitor.PC_Monitors)               // A MOBO can be assigned to only one PC
                    .HasForeignKey(pcMonitor => pcMonitor.MonitorID)
                    .OnDelete(DeleteBehavior.SetNull);                      // Deleting a MOBO removes the relationship.
            });
        }
    }
}