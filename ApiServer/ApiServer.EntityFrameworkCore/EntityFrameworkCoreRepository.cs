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

        public async Task<Item?> GetItemByKeyAsync(Guid key)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return context.Items.SingleOrDefault(_ =>  _.ItemKeys.Select(ik => ik.Key).Contains(key));
            }
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var user = await context.Users.SingleOrDefaultAsync(_ => _.UserId == userId);
                if (user != null)
                {
                    user.Password = null;
                }
                return user;
            }
        }

        public async Task<User?> GetUserAsync(string username, string password)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var user = context.Users.SingleOrDefault(_ => _.Username == username && _.Password == password);
                if (user != null)
                {
                    user.Password = null;
                }
                return user;
            }
        }

        public Task<bool> GetUserExists(string username)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                return context.Users.AnyAsync(_ => _.Username == username);
            }
        }

        public async Task<Item> SetItemAsync(Item item)
        {

            item.ItemId = Guid.NewGuid();
            
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                if (context.Users.Any(_ => _.UserId == item.UserId))
                {
                    throw new ArgumentException("User Id not valid.");
                }
                var oldItem = context.Items.SingleOrDefault(_ => _.ItemId == item.ItemId);
                if (oldItem != null)
                {
                    oldItem.Value = item.Value;
                }
                else
                {
                    context.Items.Add(item);
                }
                await context.SaveChangesAsync();
                return await context.Items.SingleAsync(_ => _.ItemId == item.ItemId);
            }
        }

        public async Task<User> SetUserAsync(User user)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var oldUser = await context.Users.SingleOrDefaultAsync(_ => _.UserId == user.UserId);
                if (oldUser != null)
                {
                    oldUser.Password = user.Password;
                }
                else
                {
                    context.Users.Add(user);
                }
                await context.SaveChangesAsync();
                return await context.Users.SingleAsync(_ => _.UserId == user.UserId);
            }
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(Guid userId)
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                var items = await context
                    .Items
                    .Where(_ => _.UserId == userId)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.Value = null;
                }
                return items;
            }
        }
    }
}