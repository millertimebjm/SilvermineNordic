using ApiServer.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    [Route("users/{userId}/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Item>> Get(Guid userId)
        {
            return new List<Item>()
            {
                new Item()
                {
                    ItemId = new Guid(),
                    Identifier = "Identifier1",
                    Value = null,
                },
                new Item()
                {
                    ItemId = new Guid(),
                    Identifier = "Identifier2",
                    Value = null,
                },
            };
        }

        [HttpGet("{identifier}/{key}")]
        public async Task<Item> Get(Guid userId, Guid key)
        {
            return new Item()
            {
                UserId = new Guid(),
                Identifier = "Identifier1",
                ItemId = new Guid(),
                Value = "Value1",
            };
        }
    }
}
