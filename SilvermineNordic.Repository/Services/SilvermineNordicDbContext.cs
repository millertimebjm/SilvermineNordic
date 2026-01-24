using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;


namespace SilvermineNordic.Repository.Services
{
    public class SilvermineNordicDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }

        public DbSet<Threshold> Thresholds { get; set; }
        public DbSet<CommunicationLog> CommunicationLogs { get; set; }

        public DbSet<ReadingByZip> ReadingByZips {get; set;}
        public DbSet<Reading> Readings { get; set; }

    public SilvermineNordicDbContext(DbContextOptions<SilvermineNordicDbContext> options)
        : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reading>().ToTable("Reading");
            modelBuilder.Entity<Threshold>().ToTable("Threshold");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserOtp>().ToTable("UserOtp");
            modelBuilder.Entity<ReadingByZip>().ToTable("ReadingByZip");
        }
    }
}

