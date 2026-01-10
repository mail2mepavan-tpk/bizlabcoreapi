using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bizlabcoreapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupabaseController : ControllerBase
    {
        // GET: api/<SupabaseController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SupabaseController>/5
        [HttpGet("{tableName}")]
        public Task<string> Get(string tableName)
        {
            return new SupabaseRestClient().GetRawDataAsync(tableName);
        }

        // POST api/<SupabaseController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SupabaseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SupabaseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class SupabaseRestClient
    {
        private readonly string apiUrl = "https://atkfcwdmmwayzyixlpum.supabase.co/rest/v1/"; // Replace with your table endpoint
        private readonly string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImF0a2Zjd2RtbXdheXp5aXhscHVtIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE2NTIzODYsImV4cCI6MjA2NzIyODM4Nn0.VkFA1SIOLgddP-P_KjFdvDyiNdUSgYfbcP5UvOardKI"; // Replace with your public anon key
        private readonly HttpClient client = new HttpClient();

        public async Task<string> GetRawDataAsync(string tableName)
        {
            client.DefaultRequestHeaders.Clear();
            // Add the required headers for authentication
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            HttpResponseMessage response = await client.GetAsync(apiUrl + tableName);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                // You can deserialize the JSON string into your C# model here
                // var cities = JsonSerializer.Deserialize<List<City>>(jsonResponse);
                return jsonResponse;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}");
            }
        }
    }

}
