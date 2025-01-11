using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LocationApp.Model.Core;

public partial class RepositoryContext : DbContext
{
    public RepositoryContext()
    {
    }

    public RepositoryContext(DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<RequestResponseLog> RequestResponseLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC071ADA2698");

            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(64);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC07DA1E33D5");

            entity.ToTable("Location", tb => tb.HasTrigger("LocationUpdateDateTrigger"));

            entity.Property(e => e.Address).HasMaxLength(512);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExternalPlaceId)
                .HasMaxLength(255)
                .HasColumnName("ExternalPlaceID");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasMany(d => d.Categories).WithMany(p => p.Locations)
                .UsingEntity<Dictionary<string, object>>(
                    "LocationCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__LocationC__Categ__4CA06362"),
                    l => l.HasOne<Location>().WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__LocationC__Locat__4BAC3F29"),
                    j =>
                    {
                        j.HasKey("LocationId", "CategoryId").HasName("PK__Location__466E3737058008BC");
                        j.ToTable("LocationCategory");
                    });
        });

        modelBuilder.Entity<RequestResponseLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Response__3214EC073F86B406");

            entity.ToTable("RequestResponseLog");

            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndpointUrl).HasMaxLength(512);
            entity.Property(e => e.RequestMethod).HasMaxLength(8);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07800907A8");

            entity.ToTable("User", tb => tb.HasTrigger("UserUpdateDateTrigger"));

            entity.Property(e => e.ApiKey)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(64);

            entity.HasMany(d => d.Locations).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserLocation",
                    r => r.HasOne<Location>().WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserLocat__Locat__48CFD27E"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserLocat__UserI__47DBAE45"),
                    j =>
                    {
                        j.HasKey("UserId", "LocationId").HasName("PK__UserLoca__79F72605C93A2FB2");
                        j.ToTable("UserLocation");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
