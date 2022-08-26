using ApiServer.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.EntityFrameworkCore
{
    public partial class ApiServerDbContext : DbContext
    {
        public ApiServerDbContext()
        {

        }

        public ApiServerDbContext(DbContextOptions<ApiServerDbContext> options)
            : base(options)
        {

        }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasOne(i => i.User)
                    .WithMany(u => u.Items)
                    .HasForeignKey(i => i.ItemId);
                    //.HasConstraintName("FK_Board_Player");
            });
        }
    }
}
