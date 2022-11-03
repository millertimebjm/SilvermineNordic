using ApiServer.EntityFrameworkCore;
using ApiServer.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServer.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRepository _repository;

        public ValuesController()
        {
            _repository = new EntityFrameworkCoreRepository("asdf");
        }

        //// GET: api/<ValuesController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<ValuesController>/5
        [HttpGet("{identifier}/{key}")]
        public async Task<IActionResult> Get(Guid key)
        {
            var value = await _repository.GetItemValueByKeyAsync(key);
            if (value != null)
            {
                return Ok(value);
            }
            return NotFound();
        }

        //// POST api/<ValuesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{

        //}

        //// PUT api/<ValuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ValuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
