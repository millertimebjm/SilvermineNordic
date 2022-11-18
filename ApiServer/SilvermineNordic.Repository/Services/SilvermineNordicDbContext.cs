using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilvermineNordic.Repository.Services
{
    public class SilvermineNordicDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }
      
        public DbSet<Threshold> Thresholds { get; set; }
        public DbSet<CommunicationLog> CommunicationLogs { get; set; }

   
        public DbSet<SensorReading> SensorReadings { get; set; }
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public SilvermineNordicDbContext(IConfiguration configuration)
        : base()
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetSqlConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorReading>().ToTable("SensorReading");
            modelBuilder.Entity<Threshold>().ToTable("SensorThreshold");
        }
    }
}

