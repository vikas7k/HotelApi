using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Models;

public partial class HotelBookingContext : DbContext
{
    public HotelBookingContext()
    {
    }

    public HotelBookingContext(DbContextOptions<HotelBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings).HasForeignKey(d => d.RoomId);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(10);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms).HasForeignKey(d => d.HotelId);

            entity.HasOne(d => d.Type).WithMany(p => p.Rooms).HasForeignKey(d => d.TypeId);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.Property(e => e.Type).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
