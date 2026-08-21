using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<string>();

                // Relationships
                entity.HasOne(b => b.Customer).WithMany(c => c.Bookings).HasForeignKey(b => b.CustomerId);
                entity.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId);
                entity.HasOne(b => b.Hotel).WithMany(h => h.Booking).HasForeignKey(b => b.HotelId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FuullName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(11);
                entity.Property(e => e.NationalCode).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.NationalCode).IsUnique();
                entity.HasMany(c => c.Bookings).WithOne(b => b.Customer).HasForeignKey(b => b.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.User).WithOne(u => u.Customer).HasForeignKey<Customer>(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            // Room Configuration
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Hotel).WithMany(c => c.Rooms);
                entity.HasMany(r => r.Bookings).WithOne(b => b.Room).HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.RoomNumber).IsUnique();
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Address).IsRequired().HasMaxLength(500);
                entity.Property(h => h.City).IsRequired().HasMaxLength(100);
                entity.Property(h => h.Country).HasMaxLength(50);
                entity.Property(h => h.PhoneNumber).HasMaxLength(20);
                entity.Property(h => h.Email).HasMaxLength(100);
                entity.Property(h => h.Description).HasMaxLength(1000);
                entity.HasMany(h => h.Rooms).WithOne(r => r.Hotel).HasForeignKey(r => r.RoomNumber).OnDelete(DeleteBehavior.Restrict);
                
            });
        }
    }
}
