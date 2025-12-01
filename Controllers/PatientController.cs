using bizlabcoreapi.Data;
using bizlabcoreapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bizlabcoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        // GET: api/<PatientController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new Data.PatientData().GetPatients();
            return Ok(response);
        }

        // GET api/<PatientController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PatientController>
        [HttpPost]
        public void Post(Models.Patient patientData)
        {
            patientData.Id = Guid.NewGuid();
            new Data.PatientData().InsertPatient(patientData);
        }

        // PUT api/<PatientController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpPut("inactive/{id}")]
        public void Inactive(string id)
        {
            new Data.PatientData().UpdatePatientInactive(id);
        }

        // DELETE api/<PatientController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            new Data.PatientData().DeletePatient(id);
        }
    }
}
