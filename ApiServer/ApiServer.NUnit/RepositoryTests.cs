using ApiServer.EntityFrameworkCore;
using ApiServer.Model;
using ApiServer.Repository;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ApiServer.NUnit
{
    public class RepositoryTests
    {
        private readonly IRepository _repository;
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
            ItemKeys = new List<ItemKey>(),
        };
        private Item Item2 = new Item()
        {
            User = null,
            ItemId = Guid.NewGuid(),
            Identifier = "Item2Identifier",
            Value = "Item2Value",
        };
        private ItemKey ItemKey1 = new ItemKey()
        {
            ItemKeyId = Guid.NewGuid(),
            Note = "ItemKey1 Note",
            Key = Guid.NewGuid(),
        };

        public RepositoryTests()
        {
             _repository = new EntityFrameworkCoreRepository("RepositoryTests");
            var optionsBuilder = new DbContextOptionsBuilder<ApiServerDbContext>();
            //optionsBuilder.UseSqlServer(@"Server=localhost;Database=Card.Dev;Trusted_Connection=True;");
            optionsBuilder.UseInMemoryDatabase("RepositoryTests");
            _contextOptions = optionsBuilder.Options;

            SeedData();
        }

        [SetUp]
        public void Setup()
        {
            
        }

        private void SeedData()
        {
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                Item1.ItemKeys.Add(ItemKey1);
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
        public async Task GetUserAsync_UserId()
        {
            User repositoryUser = await _repository.GetUserAsync(User1.UserId.Value);
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                User dbUser = context.Users.Single(_ => _.UserId == User1.UserId);
                Assert.AreEqual(repositoryUser, dbUser);
                Assert.IsTrue(repositoryUser.Password == null);
            }
        }

        [Test]
        public async Task GetUserAsync_UsernamePassword()
        {
            User repositoryUser = await _repository.GetUserAsync(User1.Username, User1.Password);
            using (var context = new ApiServerDbContext(_contextOptions))
            {
                User dbUser = context.Users.Single(_ => _.UserId == User1.UserId);
                Assert.AreEqual(repositoryUser, dbUser);
                Assert.IsTrue(repositoryUser.Password == null);
            }
        }

        [Test]
        public async Task GetItemsAsync_UserId()
        {
            var repositoryItems = await _repository.GetItemsAsync(User1.UserId.Value);
            Assert.IsTrue(repositoryItems.Contains(Item1));
            foreach (var item in repositoryItems)
            {
                Assert.IsTrue(item.Value == null);
                Assert.IsTrue(item.UserId == new Guid());
            }
        }

        [Test]
        public async Task GetItemAsync_ItemId()
        {
            var value = await _repository.GetItemValueByKeyAsync(ItemKey1.ItemKeyId);
            Assert.AreEqual(value, Item1.Value);
        }

        [Test]
        public async Task SetUserAsync_NewUser()
        {
            var user = new User()
            {
                UserId = new Guid(),
                Username = "SetUserAsync_NewUser",
                Password = "SetUserAsync_NewUser",
            };
            var copiedUser = new User()
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = user.Password,
            };
            var insertedUser = await _repository.SetUserAsync(copiedUser);
            Assert.AreEqual(user.Username, insertedUser.Username);
            Assert.AreNotEqual(user.UserId, insertedUser.UserId);
        }

        [Test]
        public async Task SetUserAsync_NoPresetUserId()
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                Username = "SetUserAsync_NoPresetUserId",
                Password = "SetUserAsync_NoPresetUserId",
            };
            var copiedUser = new User()
            {
                UserId = user.UserId,
                Username = user.Username,
                Password = user.Password,
            };
            var insertedUser = await _repository.SetUserAsync(copiedUser);
            Assert.AreNotEqual(user.UserId, insertedUser.UserId);
        }

        [Test]
        public async Task SetUserAsync_PasswordNotReturned()
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                Username = "SetUserAsync_NoPresetUserId",
                Password = "SetUserAsync_NoPresetUserId",
            };
            var insertedUser = await _repository.SetUserAsync(user);
            Assert.IsNull(user.Password);
        }
    }
}
