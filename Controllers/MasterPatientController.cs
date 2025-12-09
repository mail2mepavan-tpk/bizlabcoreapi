using bizlabcoreapi.Data;
using bizlabcoreapi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bizlabcoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterPatientController : ControllerBase
    {
        // GET: api/<MasterPatientController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MasterPatientController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MasterPatientController>
        [HttpPost]
        public async Task<IActionResult> Post(MasterPatient value)
        {
          return Ok(new PatientData().InsertMasterPatient(value));
        }

        // PUT api/<MasterPatientController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MasterPatientController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
