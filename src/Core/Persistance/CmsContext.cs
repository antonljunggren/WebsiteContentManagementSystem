using Core.Models;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistance
{
    public sealed class CmsContext : DbContext
    {
        public CmsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Photograph> Photographs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("website-content");

            modelBuilder.Entity<Photograph>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Photograph>()
                .HasPartitionKey(p => p.Id);

            modelBuilder.Entity<Photograph>()
                .Property<bool>("IsActive");

            modelBuilder.Entity<Photograph>()
                .Property<DateTime>("CreatedDate");

            modelBuilder.Entity<Photograph>()
                .Property<DateTime>("ModifiedDate");

            modelBuilder.Entity<Photograph>()
                .HasQueryFilter(p => EF.Property<bool>(p, "IsActive") == true);

            modelBuilder.Entity<Photograph>()
                .Property(p => p.DateTaken)
                .HasConversion<EfDateOnlyConverter>();

            modelBuilder.Entity<Photograph>()
                .OwnsOne(p => p.Location,
                    navBuilder =>
                    {
                        navBuilder.Property(l => l.Country);
                        navBuilder.Property(l => l.Province);
                        navBuilder.Property(l => l.City);
                    });

            modelBuilder.Entity<Photograph>()
                .OwnsOne(p => p.HdImageData,
                    navBuilder =>
                    {
                        navBuilder.Property(p => p.ImageFilePath);
                        navBuilder.Property(p => p.FileContentType);
                    });

            modelBuilder.Entity<Photograph>()
                .OwnsOne(p => p.SdImageData,
                    navBuilder =>
                    {
                        navBuilder.Property(p => p.ImageFilePath);
                        navBuilder.Property(p => p.FileContentType);
                    });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entry in ChangeTracker.Entries().Where(e => e.Entity is Photograph))
            {
                var act = entry.CurrentValues["IsActive"];
                var crea = entry.CurrentValues["CreatedDate"];
                var mod = entry.CurrentValues["ModifiedDate"];

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsActive"] = true;
                        entry.CurrentValues["CreatedDate"] = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues["ModifiedDate"] = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsActive"] = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
