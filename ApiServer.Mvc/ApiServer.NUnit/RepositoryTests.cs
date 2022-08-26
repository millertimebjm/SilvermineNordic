using ApiServer.EntityFrameworkCore;
using ApiServer.Model;
using ApiServer.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.NUnit
{
    public class RepositoryTests
    {
        private DbContextOptions<ApiServerDbContext> _contextOptions;
        private User User1 = new User()
        {
            Items = new List<Item>(),
            UserId = Guid.NewGuid(),
            Username = "User1Username",
            Password = "User1Password",
        };
        private Item Item1 = new Item()
        {
            User = null,
            ItemId = Guid.NewGuid(),
            Identifier = "Item1Identifier",
            Value = "Item1Value",
        };
        private Item Item2 = new Item()
        {
            User = null,
            ItemId = Guid.NewGuid(),
            Identifier = "Item2Identifier",
            Value = "Item2Value",
        };

        [SetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiServerDbContext>();
            //optionsBuilder.UseSqlServer(@"Server=localhost;Database=Card.Dev;Trusted_Connection=True;");
            optionsBuilder.UseInMemoryDatabase("RepositoryTests");
            _contextOptions = optionsBuilder.Options;

            SeedData();
        }

        private void SeedData()
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                User1.Items.Add(Item1);
                context.Users.AddRange(new List<User>()
                {
                    User1,
                });
                context.Items.AddRange(new List<Item>()
                {
                    Item2,
                });
                context.SaveChanges();
            }
        }

        [Test]
        public async Task GetUserAsync()
        {
            IRepository repository = new EntityFrameworkCoreRepository("RepositoryTests");
            User repositoryUser = await repository.GetUserAsync(User1.UserId);
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                User dbUser = context.Users.Single(_ => _.UserId == User1.UserId);
                Assert.AreEqual(repositoryUser, dbUser);
            }
        }
    }
}
