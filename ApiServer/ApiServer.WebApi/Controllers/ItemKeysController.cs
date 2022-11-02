using ApiServer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    [Route("users/{userId}/items/{identifier}/[controller]")]
    [ApiController]
    public class ItemKeysController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ItemKey>> Get(Guid userId, string identifier)
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
    }
}
