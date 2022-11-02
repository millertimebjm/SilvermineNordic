using ApiServer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<User> Get(Guid userId)
        {
            return new User()
            {
                UserId = new Guid(),
                Username = new Guid().ToString(),
                Password = null,
            };
        }

        [HttpPost]
        public async Task<User> Post([FromBody] string username, [FromBody] string password)
        {
            return new User()
            {
                Username = username,
                Password = null,
                UserId = new Guid(),
            };
        }
    }
}
