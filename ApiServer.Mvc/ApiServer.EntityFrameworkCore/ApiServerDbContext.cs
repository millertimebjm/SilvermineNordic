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
        public DbSet<ItemKey> ItemKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasOne(u => u.User)
                    .WithMany(i => i.Items)
                    .HasForeignKey(u => u.UserId);
                //.HasConstraintName("FK_Board_Player");

                entity.HasMany(ik => ik.ItemKeys)
                    .WithOne(i => i.Item)
                    .HasForeignKey(ik => ik.ItemId);
            });
        }
    }
}
