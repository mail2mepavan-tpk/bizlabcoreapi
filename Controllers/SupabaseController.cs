using Azure.Core;
using bizlabcoreapi.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

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

        /// <summary>
        ///  Data Sync TableNames Below:
        ///  
        ///  biosync_email_queue
        ///  biosync_email_verifications
        ///  master_patient_uploads 
        ///  daily_entries
        ///  patient_forms
        ///  service_summaries
        ///  cafe_order_item_mods
        ///  cafe_order_items
        ///  cafe_orders
        ///  master_patients
        ///  patient_check_ins
        ///  
        /// </summary>
        // GET api/<SupabaseController>/5
        [HttpGet("{tableName}")]
        public string Get(string tableName)
        {
            var jsonData = new SupabaseRestClient().GetRawDataAsync(tableName.ToString());
            switch (tableName.ToString())
            {
                case "biosync_email_queue":
                    return new BioSyncEmailQueueData().InsertData(jsonData);
                    break;
                case "biosync_email_verifications":
                    return new BioSyncEmailVerificationsData().InsertData(jsonData);
                    break;
                case "master_patient_uploads":
                    return new MasterPatientsUploadData().InsertData(jsonData);
                    break;
                case "daily_entries":
                    return new DailyEntriesData().InsertData(jsonData);
                    break;
                case "patient_forms":
                    return new PatientFormData().InsertData(jsonData);
                    break;
                case "service_summaries":
                    return new ServiceSummariesData().InsertData(jsonData);
                    break;
                case "cafe_order_item_mods":
                    return new CafeOrderItemMods().InsertData(jsonData);
                    break;
                case "cafe_order_items":
                    return new CafeOrderItems().InsertData(jsonData);
                    break;
                case "cafe_orders":
                    return new CafeOrderData().InsertData(jsonData);
                    break;
                case "master_patients":
                    return new PatientData().InsertData(jsonData);
                    break;
                case "patient_check_ins":
                    return new PatientCheckInsData().InsertData(jsonData);
                    break;
                default:
                    break;
            }
            return "0";
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

        public string GetRawDataAsync(string tableName)
        {
            var range = "0-10000";
            client.DefaultRequestHeaders.Clear();
            // Add the required headers for authentication
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Range", $"{range}");
            if(tableName == "service_summaries" || tableName == "daily_entries")
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Profile", $"accounting");
            }
            Task<string> response = client.GetStringAsync(apiUrl + tableName + "?select=*");
            string jsonResponse = response.Result;
            return jsonResponse;
        }
    }

    public enum TableName
    {
        biosync_email_queue,
        biosync_email_verifications,
        master_patient_uploads ,
        daily_entries,
        patient_forms,
        service_summaries,
        cafe_order_item_mods,
        cafe_order_items,
        cafe_orders,
        master_patients,
        patient_check_ins
    }

}
