using bizlabcoreapi.Controllers;
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
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
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

        public string InsertData(dynamic jsonData)
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

            return cafeRecs.Count.ToString();
        }
    }

    public class PatientCheckInsData
    {
        private readonly IConfiguration _configuration;

        public PatientCheckInsData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


            var sql = @"
INSERT INTO patient_check_ins (
    id,
    patient_id,
    patient_name,
    check_in_time,
    check_out_time,
    location,
    service_type,
    status,
    provider,
    created_at,
    updated_at,
    business_date,
    source,
    device_id,
    version,
    last_modified_by,
    locked_by,
    lock_expires_at,
    deleted_at,
    phone,
    email,
    notes,
    sub_service,
    date_in,
    time_in,
    first_visit,
    service,
    sub_reason,
    gift_card,
    comments
)
VALUES (
    @id,
    @patient_id,
    @patient_name,
    @check_in_time,
    @check_out_time,
    @location,
    @service_type,
    @status,
    @provider,
    @created_at,
    @updated_at,
    @business_date,
    @source,
    @device_id,
    @version,
    @last_modified_by,
    @locked_by,
    @lock_expires_at,
    @deleted_at,
    @phone,
    @email,
    @notes,
    @sub_service,
    @date_in,
    @time_in,
    @first_visit,
    @service,
    @sub_reason,
    @gift_card,
    @comments
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from patient_check_ins", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic checkIns = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < checkIns.Count; i++)
            {
                var checkIn = checkIns[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", Guid.Parse(checkIn.id.Value));
                    cmd.Parameters.AddWithValue("patient_id", checkIn.patient_id.Value != null ? checkIn.patient_id.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("patient_name", checkIn.patient_name.Value != null ? checkIn.patient_name.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("check_in_time", checkIn.check_in_time.Value != null ? DateTimeOffset.Parse(checkIn.check_in_time.Value.ToString()) : DBNull.Value);
                    cmd.Parameters.AddWithValue("check_out_time", checkIn.check_out_time.Value != null ? DateTimeOffset.Parse(checkIn.check_out_time.Value.ToString()) : DBNull.Value);
                    cmd.Parameters.AddWithValue("location", checkIn.location.Value != null ? checkIn.location.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("service_type", checkIn.service_type.Value != null ? checkIn.service_type.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("status", checkIn.status.Value != null ? checkIn.status.Value : DBNull.Value );
                    cmd.Parameters.AddWithValue("provider", DBNull.Value);
                    cmd.Parameters.AddWithValue("created_at", DateTimeOffset.Parse(checkIn.created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("updated_at", DateTimeOffset.Parse(checkIn.updated_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("business_date", checkIn.business_date.Value != null ? DateTimeOffset.Parse(checkIn.business_date.Value.ToString()).Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("source", checkIn.source.Value != null ? checkIn.source.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("device_id", checkIn.device_id.Value != null ? checkIn.device_id.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("version", checkIn.version.Value != null ? checkIn.version.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("last_modified_by", checkIn.last_modified_by.Value != null ? checkIn.last_modified_by.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("locked_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("lock_expires_at", DBNull.Value);
                    cmd.Parameters.AddWithValue("deleted_at", DBNull.Value);
                    cmd.Parameters.AddWithValue("phone", checkIn.phone.Value != null ? checkIn.phone.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("email", checkIn.email.Value != null ? checkIn.email.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("notes", checkIn.notes.Value != null ? checkIn.notes.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("sub_service", DBNull.Value);
                    cmd.Parameters.AddWithValue("date_in", DBNull.Value);
                    cmd.Parameters.AddWithValue("time_in", DBNull.Value);
                    cmd.Parameters.AddWithValue("first_visit", checkIn.first_visit.Value != null ? checkIn.first_visit.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("service", DBNull.Value);
                    cmd.Parameters.AddWithValue("sub_reason", DBNull.Value);
                    cmd.Parameters.AddWithValue("gift_card", DBNull.Value);
                    cmd.Parameters.AddWithValue("comments", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return checkIns.Count.ToString();
        }
    }

    public class CafeOrderItemMods
    {
        private readonly IConfiguration _configuration;

        public CafeOrderItemMods()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            string sql = @"
INSERT INTO cafe_order_item_mods (
    id,
    order_item_id,
    modifier_id,
    modifier_name_snapshot,
    price_delta_cents,
    created_at
)
VALUES (
    @id,
    @order_item_id,
    @modifier_id,
    @modifier_name_snapshot,
    @price_delta_cents,
    @created_at
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from cafe_order_item_mods", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic cafeRecs = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < cafeRecs.Count; i++)
            {
                var cafeItemMod = cafeRecs[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(cafeItemMod.id.Value));
                    cmd.Parameters.AddWithValue("@order_item_id", Guid.Parse(cafeItemMod.order_item_id.Value));
                    cmd.Parameters.AddWithValue("@modifier_id", Guid.Parse(cafeItemMod.modifier_id.Value));
                    cmd.Parameters.AddWithValue("@modifier_name_snapshot", cafeItemMod.modifier_name_snapshot.Value);
                    cmd.Parameters.AddWithValue("@price_delta_cents", cafeItemMod.price_delta_cents.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(cafeItemMod.created_at.Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            return cafeRecs.Count.ToString();
        }
    }

    public class CafeOrderItems
    {
        private readonly IConfiguration _configuration;

        public CafeOrderItems()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            string sql = @"
INSERT INTO cafe_order_items (
    id,
    order_id,
    item_id,
    item_name_snapshot,
    unit_price_cents,
    quantity,
    line_subtotal_cents,
    line_total_cents,
    created_at
)
VALUES (
    @id,
    @order_id,
    @item_id,
    @item_name_snapshot,
    @unit_price_cents,
    @quantity,
    @line_subtotal_cents,
    @line_total_cents,
    @created_at
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from cafe_order_items", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic cafeRecs = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < cafeRecs.Count; i++)
            {
                var cafeItem = cafeRecs[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(cafeItem.id.Value));
                    cmd.Parameters.AddWithValue("@order_id", Guid.Parse(cafeItem.order_id.Value));
                    cmd.Parameters.AddWithValue("@item_id", Guid.Parse(cafeItem.item_id.Value));
                    cmd.Parameters.AddWithValue("@item_name_snapshot", cafeItem.item_name_snapshot.Value);
                    cmd.Parameters.AddWithValue("@unit_price_cents", cafeItem.unit_price_cents.Value);
                    cmd.Parameters.AddWithValue("@quantity", cafeItem.quantity.Value);
                    cmd.Parameters.AddWithValue("@line_subtotal_cents", cafeItem.line_subtotal_cents.Value);
                    cmd.Parameters.AddWithValue("@line_total_cents", cafeItem.line_total_cents.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(cafeItem.created_at.Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            return cafeRecs.Count.ToString();
        }
    }

    public class ServiceSummariesData
    {
        private readonly IConfiguration _configuration;

        public ServiceSummariesData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            string sql = @"
INSERT INTO accounting.service_summaries (
    id,
    daily_entry_id,
    summary_type,
    summary_key,
    patient_count,
    service_count,
    total_revenue,
    total_tips,
    total_amount,
    metadata,
    entry_date,
    location_id,
    created_at
)
VALUES (
    @id,
    @daily_entry_id,
    @summary_type,
    @summary_key,
    @patient_count,
    @service_count,
    @total_revenue,
    @total_tips,
    @total_amount,
    @metadata,
    @entry_date,
    @location_id,
    @created_at
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from accounting.service_summaries", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic cafeRecs = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < cafeRecs.Count; i++)
            {
                var summaryItem = cafeRecs[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(summaryItem.id.Value));
                    cmd.Parameters.AddWithValue("@daily_entry_id", Guid.Parse(summaryItem.daily_entry_id.Value));
                    cmd.Parameters.AddWithValue("@summary_type", summaryItem.summary_type.Value);
                    cmd.Parameters.AddWithValue("@summary_key", summaryItem.summary_key.Value);
                    cmd.Parameters.AddWithValue("@patient_count", summaryItem.patient_count.Value);
                    cmd.Parameters.AddWithValue("@service_count", summaryItem.service_count.Value);
                    cmd.Parameters.AddWithValue("@total_revenue", summaryItem.total_revenue.Value);
                    cmd.Parameters.AddWithValue("@total_tips", summaryItem.total_tips.Value);
                    cmd.Parameters.AddWithValue("@total_amount", summaryItem.total_amount.Value);
                    cmd.Parameters.AddWithValue("@metadata", DBNull.Value);
                    cmd.Parameters.AddWithValue("@entry_date", DateTime.Parse(summaryItem.entry_date.Value.ToString()).Date);
                    cmd.Parameters.AddWithValue("@location_id", summaryItem.location_id.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(summaryItem.created_at.Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            return cafeRecs.Count.ToString();
        }
    }

    public class PatientFormData
    {
        private readonly IConfiguration _configuration;

        public PatientFormData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            string sql = @"
INSERT INTO patient_forms (
    id,
    patient_id,
    form_type,
    form_data,
    uploaded_files,
    submitted_at,
    status,
    reviewed_by,
    reviewed_at,
    notes,
    created_at,
    updated_at,
    raw_data,
    patient_name
)
VALUES (
    @id,
    @patient_id,
    @form_type,
    @form_data,
    @uploaded_files,
    @submitted_at,
    @status,
    @reviewed_by,
    @reviewed_at,
    @notes,
    @created_at,
    @updated_at,
    @raw_data,
    @patient_name
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from patient_forms", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic cafeRecs = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < cafeRecs.Count; i++)
            {
                var patientForm = cafeRecs[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(patientForm.id.Value));
                    cmd.Parameters.AddWithValue("@patient_id", Guid.Parse(patientForm.patient_id.Value));
                    cmd.Parameters.AddWithValue("@form_type", patientForm.form_type.Value);

                    // form_data → jsonb
                    cmd.Parameters.Add("@form_data", NpgsqlDbType.Jsonb).Value = patientForm.form_data.ToString();
                    cmd.Parameters.Add("@uploaded_files", NpgsqlDbType.Jsonb).Value = patientForm.uploaded_files.ToString();
                    cmd.Parameters.AddWithValue("@submitted_at", DateTime.Parse(patientForm.submitted_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@status", patientForm.id.Value);
                    cmd.Parameters.AddWithValue("@reviewed_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("@reviewed_at", DBNull.Value);
                    cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(patientForm.created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@updated_at", DateTime.Parse(patientForm.updated_at.Value.ToString()));
                    cmd.Parameters.Add("@raw_data", NpgsqlDbType.Jsonb).Value = DBNull.Value;
                    cmd.Parameters.AddWithValue("@patient_name", patientForm.patient_name.Value != null ? patientForm.patient_name.Value : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return cafeRecs.Count.ToString();
        }
    }

    public class DailyEntriesData
    {
        private readonly IConfiguration _configuration;

        public DailyEntriesData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}


            string sql = @"
INSERT INTO accounting.daily_entries (
    id,
    entry_date,
    location_id,
    location_name,
    total_patients,
    new_patients,
    returning_patients,
    total_services,
    total_service_revenue,
    total_tips,
    total_cafe_sales,
    total_retail_sales,
    grand_total,
    daily_notes,
    created_at,
    created_by,
    updated_at,
    updated_by,
    export_timestamp,
    exported_by
)
VALUES (
    @id,
    @entry_date,
    @location_id,
    @location_name,
    @total_patients,
    @new_patients,
    @returning_patients,
    @total_services,
    @total_service_revenue,
    @total_tips,
    @total_cafe_sales,
    @total_retail_sales,
    @grand_total,
    @daily_notes,
    @created_at,
    @created_by,
    @updated_at,
    @updated_by,
    @export_timestamp,
    @exported_by
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from accounting.daily_entries", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic entries = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(entry.id.Value));
                    cmd.Parameters.AddWithValue("@entry_date", DateTime.Parse(entry.entry_date.Value.ToString()).Date);
                    cmd.Parameters.AddWithValue("@location_id", entry.location_id.Value);
                    cmd.Parameters.AddWithValue("@location_name", entry.location_name.Value);
                    cmd.Parameters.AddWithValue("@total_patients", entry.total_patients.Value);
                    cmd.Parameters.AddWithValue("@new_patients", entry.new_patients.Value);
                    cmd.Parameters.AddWithValue("@returning_patients", entry.returning_patients.Value);
                    cmd.Parameters.AddWithValue("@total_services", entry.total_services.Value);
                    cmd.Parameters.AddWithValue("@total_service_revenue", entry.total_service_revenue.Value);
                    cmd.Parameters.AddWithValue("@total_tips", entry.total_tips.Value);
                    cmd.Parameters.AddWithValue("@total_cafe_sales", entry.total_cafe_sales.Value);
                    cmd.Parameters.AddWithValue("@total_retail_sales", entry.total_retail_sales.Value);
                    cmd.Parameters.AddWithValue("@grand_total", entry.grand_total.Value);
                    cmd.Parameters.AddWithValue("@daily_notes", entry.daily_notes.Value != null ? entry.daily_notes.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(entry.created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@created_by", entry.created_by.Value);
                    cmd.Parameters.AddWithValue("@updated_at", DateTime.Parse(entry.updated_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@updated_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("@export_timestamp", DateTime.Parse(entry.export_timestamp.Value.ToString()));
                    cmd.Parameters.AddWithValue("@exported_by", entry.exported_by.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return entries.Count.ToString();
        }
    }

    public class MasterPatientsUploadData
    {
        private readonly IConfiguration _configuration;

        public MasterPatientsUploadData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}
            string sql = @"
INSERT INTO master_patient_uploads (
    id,
    patient_id,
    file_name,
    file_type,
    file_category,
    file_url,
    file_size,
    mime_type,
    document_date,
    description,
    is_lab_result,
    lab_provider,
    lab_ordered_by,
    results_summary,
    abnormal_findings,
    is_patient_visible,
    is_provider_only,
    uploaded_date,
    uploaded_by,
    uploaded_at,
    reviewed_by,
    reviewed_date
)
VALUES (
    @id,
    @patient_id,
    @file_name,
    @file_type,
    @file_category,
    @file_url,
    @file_size,
    @mime_type,
    @document_date,
    @description,
    @is_lab_result,
    @lab_provider,
    @lab_ordered_by,
    @results_summary,
    @abnormal_findings,
    @is_patient_visible,
    @is_provider_only,
    @uploaded_date,
    @uploaded_by,
    @uploaded_at,
    @reviewed_by,
    @reviewed_date
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from master_patient_uploads", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic entries = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(entry.id.Value));
                    cmd.Parameters.AddWithValue("@patient_id", Guid.Parse(entry.id.Value));
                    cmd.Parameters.AddWithValue("@file_name", entry.file_name.Value);
                    cmd.Parameters.AddWithValue("@file_type", entry.file_type.Value);
                    cmd.Parameters.AddWithValue("@file_category", entry.file_category.Value);
                    cmd.Parameters.AddWithValue("@file_url", entry.file_url.Value);
                    cmd.Parameters.AddWithValue("@file_size", DBNull.Value);
                    cmd.Parameters.AddWithValue("@mime_type", DBNull.Value);
                    cmd.Parameters.AddWithValue("@document_date", DateTime.Parse(entry.document_date.Value.ToString()).Date);
                    cmd.Parameters.AddWithValue("@description", entry.description.Value);
                    cmd.Parameters.AddWithValue("@is_lab_result", entry.is_lab_result.Value);
                    cmd.Parameters.AddWithValue("@lab_provider", DBNull.Value);
                    cmd.Parameters.AddWithValue("@lab_ordered_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("@results_summary", DBNull.Value);
                    cmd.Parameters.AddWithValue("@abnormal_findings", DBNull.Value);
                    cmd.Parameters.AddWithValue("@is_patient_visible", entry.is_patient_visible.Value);
                    cmd.Parameters.AddWithValue("@is_provider_only", entry.is_provider_only.Value);
                    cmd.Parameters.AddWithValue("@uploaded_date", DateTime.Parse(entry.uploaded_date.Value.ToString()).Date);
                    cmd.Parameters.AddWithValue("@uploaded_by", entry.uploaded_by.Value);
                    cmd.Parameters.AddWithValue("@uploaded_at", DateTime.Parse(entry.uploaded_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@reviewed_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("@reviewed_date", DBNull.Value);
                    //cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(entry.created_at.Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            return entries.Count.ToString();
        }
    }

    public class BioSyncEmailVerificationsData
    {
        private readonly IConfiguration _configuration;

        public BioSyncEmailVerificationsData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}
            string sql = @"
INSERT INTO biosync_email_verifications (
    id,
    email,
    submission_id,
    verification_token,
    verified,
    verified_at,
    created_at
)
VALUES (
    @id,
    @email,
    @submission_id,
    @verification_token,
    @verified,
    @verified_at,
    @created_at
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from biosync_email_verifications", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic entries = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(entry.id.Value));
                    cmd.Parameters.AddWithValue("@email", entry.email.Value != null ? entry.email.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@submission_id",entry.submission_id.Value);
                    cmd.Parameters.AddWithValue("@verification_token",entry.verification_token.Value);
                    cmd.Parameters.AddWithValue("@verified", entry.verified.Value);
                    cmd.Parameters.AddWithValue("@verified_at", DBNull.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(entry.created_at.Value.ToString()));
                    cmd.ExecuteNonQuery();
                }
            }

            return entries.Count.ToString();
        }
    }

    public class BioSyncEmailQueueData
    {
        private readonly IConfiguration _configuration;

        public BioSyncEmailQueueData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string InsertData(dynamic jsonData)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}
            string sql = @"
INSERT INTO biosync_email_queue (
    id,
    recipient,
    subject,
    html_content,
    verification_token,
    submission_id,
    status,
    created_at,
    sent_at
)
VALUES (
    @id,
    @recipient,
    @subject,
    @html_content,
    @verification_token,
    @submission_id,
    @status,
    @created_at,
    @sent_at
);
";


            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using (var cmd = new NpgsqlCommand("delete from biosync_email_queue", conn))
            {
                cmd.ExecuteNonQuery();
            }

            dynamic entries = JsonConvert.DeserializeObject<dynamic>(jsonData);
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.Parse(entry.id.Value));
                    cmd.Parameters.AddWithValue("@recipient", entry.recipient.Value);
                    cmd.Parameters.AddWithValue("@subject", entry.subject.Value);
                    cmd.Parameters.AddWithValue("@html_content", entry.html_content.Value);
                    cmd.Parameters.AddWithValue("@verification_token",entry.verification_token.Value);
                    cmd.Parameters.AddWithValue("@submission_id", entry.submission_id.Value);
                    cmd.Parameters.AddWithValue("@status", entry.status.Value);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Parse(entry.created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("@sent_at", DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }

            return entries.Count.ToString();
        }
    }

    public class DataReaderData
    {
        private readonly IConfiguration _configuration;

        public DataReaderData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public string GetData(string tableName)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            //using var supaBaseConn = new NpgsqlConnection(_configuration.GetConnectionString("SupabaseConnection"));
            //supaBaseConn.Open();
            //using (var cmd = new NpgsqlCommand("select * from cafe_orders", supaBaseConn))
            //{
            //    var res = cmd.ExecuteScalar();
            //}
           


            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();

            var cmd = new NpgsqlCommand("select * from " + tableName, conn);
            using var reader = cmd.ExecuteReader();

            var rows = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[reader.GetName(i)] = value;
                }

                rows.Add(row);
            }

            // Convert to JSON string
            string json = System.Text.Json.JsonSerializer.Serialize(
                rows,
                new JsonSerializerOptions { WriteIndented = true }
            );

            return json;
        }
    }
}

