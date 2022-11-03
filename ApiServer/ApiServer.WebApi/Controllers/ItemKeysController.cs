using ApiServer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemKeysController : ControllerBase
    {
        [HttpGet("list/{itemId}")]
        public async Task<IEnumerable<ItemKey>> List(Guid itemId)
        {
            return new List<ItemKey>()
            {
                new ItemKey()
                {
                    ItemId = new Guid(),
                    Key = new Guid(),
                    Note = "This is a key for Identifier1",
                },
            };
        }

        [HttpGet("{key}")]
        public async Task<Item> Get(Guid key)
        {
            return new Item()
            {
                ItemId = new Guid(),
                Value = "Value1",
            };
        }

        [HttpPost("{userId}/{itemId}")]
        public async Task<ItemKey> Post(Guid userId, Guid itemId, [FromBody] string note)
        {
            return new ItemKey()
            {
                ItemId = new Guid(),
                Key = new Guid(),
                Note = note,
            };
        }

        [HttpPost("{key}")]
        public async Task<Item> Post(Guid key, string value)
        {
            return new Item()
            {
                ItemId = new Guid(),
                Value = value,
                Identifier = "Identifier1",
            };
        }
    }
}
