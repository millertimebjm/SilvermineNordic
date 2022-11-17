using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Api.Models;

namespace SilvermineNordic.Api.Services
{
    public class SilvermineNordicDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<UserOtp> UserOtps { get; set; }
        public DbSet<Threshold> Thresholds { get; set; }
        public DbSet<CommunicationLog> CommunicationLogs { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }

        public SilvermineNordicDbContext(DbContextOptions<SilvermineNordicDbContext> options)
        : base(options)
        {

        }
    }
}
