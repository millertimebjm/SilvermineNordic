using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Repository.Models;

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
        private readonly ISilvermineNordicConfiguration _configuration;

        public SilvermineNordicDbContext(ISilvermineNordicConfiguration configuration)
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

