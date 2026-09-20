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

            // =========================
            // Booking
            // =========================
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.TotalPrice)
                    .HasColumnType("decimal(18,2)");

                entity.Property(b => b.Status)
                    .HasConversion<string>();

                // Customer -> Bookings
                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Room -> Bookings
                entity.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Hotel -> Bookings
                entity.HasOne(b => b.Hotel)
                    .WithMany(h => h.Booking)
                    .HasForeignKey(b => b.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // Customer
            // =========================
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.Property(c => c.NationalCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasIndex(c => c.Email)
                    .IsUnique();

                entity.HasIndex(c => c.NationalCode)
                    .IsUnique();

                // Customer -> User
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Customer)
                    .HasForeignKey<Customer>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // Room
            // =========================
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.RoomNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(r => r.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.PricePerNight)
                    .HasColumnType("decimal(18,2)");

                entity.Property(r => r.Status)
                    .HasConversion<string>();

                entity.HasIndex(r => r.RoomNumber)
                    .IsUnique();

                // Room -> Hotel
                entity.HasOne(r => r.Hotel)
                    .WithMany(h => h.Rooms)
                    .HasForeignKey(r => r.HotelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // Hotel
            // =========================
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(h => h.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(h => h.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(h => h.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(h => h.Country)
                    .HasMaxLength(50);

                entity.Property(h => h.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(h => h.Email)
                    .HasMaxLength(100);

                entity.Property(h => h.Description)
                    .HasMaxLength(1000);
            });
        }
    }
}
