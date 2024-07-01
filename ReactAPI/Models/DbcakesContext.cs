using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ReactAPI.Models;

public partial class DbcakesContext : DbContext
{
    public DbcakesContext()
    {
    }

    public DbcakesContext(DbContextOptions<DbcakesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cake> Cakes { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=CadenaSQL");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cake>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cake__3214EC07B1A08425");

            entity.ToTable("Cake");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Origin)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rating__3214EC07480B1664");

            entity.ToTable("Rating");

            entity.Property(e => e.Flavor).HasColumnName("flavor");
            entity.Property(e => e.Presentation).HasColumnName("presentation");

            entity.HasOne(d => d.Cake).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.CakeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__CakeId__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__UserId__4E88ABD4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0784FBC555");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105344CFAA445").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
