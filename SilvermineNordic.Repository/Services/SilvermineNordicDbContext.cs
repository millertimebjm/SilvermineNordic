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


        public DbSet<Reading> Readings { get; set; }
        private readonly ISilvermineNordicConfiguration _configuration;

        public SilvermineNordicDbContext(
            IOptionsSnapshot<SilvermineNordicConfigurationService> options)
        : base()
        {
            _configuration = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var sqlConnectionString = _configuration?.GetSqlConnectionString();
            if (!string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                optionsBuilder.UseSqlServer(sqlConnectionString);
                return;
            }

            var inMemoryDatabaseName = _configuration?.GetInMemoryDatabaseName()
                ?? "InMemoryDatabaseName";
            optionsBuilder.UseInMemoryDatabase(inMemoryDatabaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reading>().ToTable("Reading");
            modelBuilder.Entity<Threshold>().ToTable("Threshold");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserOtp>().ToTable("UserOtp");
        }
    }
}

