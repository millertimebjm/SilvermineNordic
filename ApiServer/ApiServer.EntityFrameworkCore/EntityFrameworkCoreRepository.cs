using ApiServer.Model;
using ApiServer.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.EntityFrameworkCore
{
    public class EntityFrameworkCoreRepository : IRepository
    {
        private readonly DbContextOptions<ApiServerDbContext> _contextOptions;
        public EntityFrameworkCoreRepository(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiServerDbContext>();
            //optionsBuilder.UseSqlServer(@"Server=localhost;Database=Card.Dev;Trusted_Connection=True;");
            optionsBuilder.UseInMemoryDatabase(connectionString);
            _contextOptions = optionsBuilder.Options;
        }

        public Item? GetItemAsync(string identifier, Guid key)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return context.Items.SingleOrDefault(_ => _.Identifier == identifier && _.ItemKeys.Select(ik => ik.Key).Contains(key));
            }
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return await context.Users.SingleOrDefaultAsync(_ => _.UserId == userId);
            }
        }

        public User? GetUserAsync(string username, string password)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return context.Users.SingleOrDefault(_ => _.Username == username && _.Password == password);
            }
        }

        public Item SetItemAsync(Item item)
        {
            if (item.ItemId == Guid.Empty)
            {
                item.ItemId = Guid.NewGuid();
            }
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var oldItem = context.Items.SingleOrDefault(_ => _.ItemId == item.ItemId);
                if (oldItem != null)
                {
                    oldItem.Value = item.Value;
                }
                else
                {
                    context.Items.Add(item);
                }
                context.SaveChanges();
                return context.Items.Single(_ => _.ItemId == item.ItemId);
            }
        }

        public User SetUserAsync(User user)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var oldUser = context.Users.SingleOrDefault(_ => _.UserId == user.UserId);
                if (oldUser != null)
                {
                    oldUser.Password = user.Password;
                }
                else
                {
                    context.Users.Add(user);
                }
                context.SaveChanges();
                return context.Users.Single(_ => _.UserId == user.UserId);
            }
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(Guid userId)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return await context
                    .Items
                    .Include(i => i.ItemKeys)
                    .Where(_ => _.UserId == userId)
                    .ToListAsync();
            }
        }
    }
}