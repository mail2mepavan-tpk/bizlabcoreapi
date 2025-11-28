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

namespace bizlabcoreapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StaffController : Controller
    {
        private readonly bizlabcoreapiContext _context;

        public StaffController(bizlabcoreapiContext context)
        {
            _context = context;
        }

        // GET: AllStaff
        [HttpGet("/allstaff")]
        public async Task<IActionResult> Get()
        {
            return Ok(GetStaffUsersDataAll());
        }

        // GET: AllStaff
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            return Ok(StaffAuthorize(username, password));
        }

        // GET: AllStaff
        [HttpPost("NewStaffUser")]
        public async Task<IActionResult> NewStaffUser(staff_users staff)
        {
            return Ok(CreateStaffUser(staff));
        }

        [HttpDelete("DeleteStaffUser")]
        public async Task<IActionResult> DeleteStaffUser(string staffid)
        {
            return Ok(DeleteUser(staffid));
        }

        private string GetStaffUsersDataAll()
        {
            var sqlCommand = "select id,username,full_name, password_hash,email, role, location, active from staff_users";
            var results = new List<Dictionary<string, object>>();

            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(sqlCommand, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            string json = JsonConvert.SerializeObject(results, Formatting.Indented);
            return json;
        }

        private string CreateStaffUser(staff_users user)
        {
            string json = "";
            string insertQuery = @"
                INSERT INTO public.staff_users
                    (id, username, password_hash, email, full_name, role, location, active, created_at)
                VALUES
                    (@id, @username, @password_hash, @Email, @full_name, @role, @location, @active, @created_at)
                RETURNING id;";

            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", System.Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@password_hash", user.password_hash);
                    cmd.Parameters.AddWithValue("@email", user.email);
                    cmd.Parameters.AddWithValue("@full_name", user.full_name);
                    cmd.Parameters.AddWithValue("@role", user.role);
                    cmd.Parameters.AddWithValue("@location", user.location);
                    cmd.Parameters.AddWithValue("@active", user.active);
                    cmd.Parameters.AddWithValue("@created_at", user.LastUpdate);

                    // Execute and get the generated ID (if table has SERIAL or IDENTITY)
                    Guid insertedId = (Guid)cmd.ExecuteScalar();
                    json = JsonConvert.SerializeObject(insertedId.ToString(), Formatting.Indented);
                }
            }
            return json;
        }

        private string DeleteUser(string id)
        {
            string json = "";
            string insertQuery = @"DELETE FROM public.staff_users WHERE id = @id";

            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(id));

                    // Execute and get the generated ID (if table has SERIAL or IDENTITY)
                    int rowsAffected = cmd.ExecuteNonQuery();
                    json = JsonConvert.SerializeObject(rowsAffected.ToString(), Formatting.Indented);
                }
            }
            return json;
        }

        private string StaffAuthorize(string username, string password)
        {
            var sqlCommand = "select id,username,full_name, password_hash,email, role, location, active from staff_users " +
                "where username='" + username + "' and password_hash='" + password + "'";
            var results = new List<Dictionary<string, object>>();
            using (var connection = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(sqlCommand, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            if(results.Count > 0)
            {
                // Successful authentication logic can be added here
                string json = JsonConvert.SerializeObject(results, Formatting.Indented);
                return json;
            }
            else
            {
                return null;
            }

        }
    }
}

    