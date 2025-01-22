using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessWebApp.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FitnessWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<Uzivatel>
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect())
                    {
                        databaseCreator.Create();
                    }
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public DbSet<FitnessWebApp.Models.TP> TP { get; set; } = default!;
        public DbSet<FitnessWebApp.Models.DenVTydnu> DenVTydnu { get; set; }
        public DbSet<FitnessWebApp.Models.DenTreninku> DenTreninku { get; set; }
        public DbSet<FitnessWebApp.Models.Cvik> Cvik { get; set; } = default!;
        public DbSet<FitnessWebApp.Models.TreninkoveData> TreninkoveData { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Cvik>()
                .HasIndex(c => new { c.Nazev, c.UzivatelId })
                .IsUnique();

            modelBuilder.Entity<TP>()
                .HasMany(tp => tp.DnyVTydnu)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
