using bizlabcoreapi.Data;
using bizlabcoreapi.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace bizlabcoreapi.Data
{
    public class CafeOrderData
    {
        private readonly IConfiguration _configuration;

        public CafeOrderData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public void InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            var sql = @"
                    INSERT INTO cafe_orders (
                        id,
                        order_number,
                        location_id,
                        channel,
                        customer_name,
                        customer_phone,
                        customer_info,
                        notes,
                        subtotal_cents,
                        tax_cents,
                        total_cents,
                        status,
                        payment_status,
                        payment_intent_id,
                        stripe_payment_id,
                        paid_at,
                        created_at,
                        updated_at,
                        completed_at,
                        patient_id,
                        reserve_applied_cents
                    ) VALUES (
                        @id,
                        @order_number,
                        @location_id,
                        @channel,
                        @customer_name,
                        @customer_phone,
                        @customer_info,
                        @notes,
                        @subtotal_cents,
                        @tax_cents,
                        @total_cents,
                        @status,
                        @payment_status,
                        @payment_intent_id,
                        @stripe_payment_id,
                        @paid_at,
                        @created_at,
                        @updated_at,
                        @completed_at,
                        @patient_id,
                        @reserve_applied_cents
                    );";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
                using (var cmd = new NpgsqlCommand("delete from cafe_orders", conn))
                { 
                    cmd.ExecuteNonQuery();
                }

                dynamic cafeRecs = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i=0;  i < cafeRecs.Count; i++) {
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", Guid.Parse(cafeRecs[i].id.Value));
                    cmd.Parameters.AddWithValue("order_number", cafeRecs[i].order_number.Value);
                    cmd.Parameters.AddWithValue("location_id", Guid.Parse(cafeRecs[i].location_id.Value));
                    cmd.Parameters.AddWithValue("channel", cafeRecs[i].channel.Value);
                    cmd.Parameters.AddWithValue("subtotal_cents", cafeRecs[i].subtotal_cents.Value);
                    cmd.Parameters.AddWithValue("tax_cents", cafeRecs[i].tax_cents.Value);
                    cmd.Parameters.AddWithValue("total_cents", cafeRecs[i].total_cents.Value);
                    cmd.Parameters.AddWithValue("status", cafeRecs[i].status.Value);
                    cmd.Parameters.AddWithValue("payment_status", cafeRecs[i].payment_status.Value);
                    cmd.Parameters.AddWithValue("reserve_applied_cents", cafeRecs[i].reserve_applied_cents.Value);

                    // Optional / nullable
                    cmd.Parameters.AddWithValue("customer_name", cafeRecs[i].customer_name.Value);
                    cmd.Parameters.AddWithValue("customer_phone", DBNull.Value);
                    cmd.Parameters.AddWithValue("customer_info", DBNull.Value);
                    cmd.Parameters.AddWithValue("notes", cafeRecs[i].notes.Value);
                    cmd.Parameters.AddWithValue("payment_intent_id", cafeRecs[i].payment_intent_id.Value != null ? cafeRecs[i].payment_intent_id.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("stripe_payment_id", cafeRecs[i].stripe_payment_id.Value != null ? cafeRecs[i].stripe_payment_id.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("paid_at", cafeRecs[i].paid_at.Value != null ? DateTimeOffset.Parse(cafeRecs[i].paid_at.Value.ToString()) : DBNull.Value);
                    cmd.Parameters.AddWithValue("created_at", DateTimeOffset.Parse(cafeRecs[i].created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("updated_at", DateTimeOffset.Parse(cafeRecs[i].updated_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("completed_at", DateTimeOffset.Parse(cafeRecs[i].completed_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("patient_id", cafeRecs[i].patient_id.Value != null ? cafeRecs[i].patient_id.Value : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
