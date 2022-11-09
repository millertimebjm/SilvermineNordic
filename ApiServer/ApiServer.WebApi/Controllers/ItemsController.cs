using ApiServer.Model;
using ApiServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository _repository;

        public ItemsController(IRepository repository) : base()
        {
            _repository = repository;
        }

        [HttpGet("list/{userId}")]
        public async Task<IEnumerable<Item>> List(Guid userId)
        {
            return await _repository.GetItemsAsync(userId);
        }

        [HttpGet("{key}")]
        public async Task<string?> Get(Guid key)
        {
            return await _repository.GetItemValueByKeyAsync(key);
        }

        [HttpPost("{userId}")]
        public async Task<Item> Post(Guid userId, [FromBody] string identifier)
        {
            return await _repository.SetItemAsync(new Item()
            {
                UserId = userId,
                Identifier = identifier,
                Value = null,
            });
        }
    }
}
