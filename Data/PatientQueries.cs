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
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace bizlabcoreapi.Data
{
    public class PatientData
    {
        private readonly IConfiguration _configuration;

        public PatientData()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            _configuration = configuration;
        }
        //private readonly bizlabcoreapiContext _context;

        public static string InsertPatientSql = @"
        INSERT INTO patients (
            id, client_id, legacy_id,
            first_name, last_name, name, email, phone,
            address_street, address_city, address_state, address_zip,
            patient_status, account_status,
            nickname, title, birth_date, company, email_opt_in, sms_opt_in,
            memo, cancellation_list, first_clinical_service_complete,
            first_clinical_service_date, membership_status, membership_start_date,
            membership_type, phone_work, phone_mobile,
            notes, created_at, updated_at
        )
        VALUES (
            @id, @client_id, @legacy_id,
            @first_name, @last_name, @name, @email, @phone,
            @address_street, @address_city, @address_state, @address_zip,
            @patient_status, @account_status,
            @nickname, @title, @birth_date, @company, @email_opt_in, @sms_opt_in,
            @memo, @cancellation_list, @first_clinical_service_complete,
            @first_clinical_service_date, @membership_status, @membership_start_date,
            @membership_type, @phone_work, @phone_mobile,
            @notes, @created_at, @updated_at
        );
    ";

        public void DeletePatient(string id, string authId)
        {
            var secureUserData = new SecureUserData();
            if (secureUserData.IsSecure(authId))
            {
                var connString = _configuration.GetConnectionString("bizlabcoreapiContext");
                var results = new List<Dictionary<string, object>>();
                using (var connection = new NpgsqlConnection(connString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("DELETE FROM patients where id='" + id + "'", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        int recs = cmd.ExecuteNonQuery();
                    }
                }
            }
            else {
                throw new UnauthorizedAccessException("Authentication Failed");
            }
        }

        public void UpdatePatientInactive(string id)
        {
            var connString = _configuration.GetConnectionString("bizlabcoreapiContext");
            var results = new List<Dictionary<string, object>>();
            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand("UPDATE patients set patient_status='inactive' where id='" + id + "'", connection))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    int recs = cmd.ExecuteNonQuery();
                }
            }
        }

        public string GetPatients(string authId)
        {
            var secureUserData = new SecureUserData();
            if (secureUserData.IsSecure(authId))
            {
                var connString = _configuration.GetConnectionString("bizlabcoreapiContext");
                var results = new List<Dictionary<string, object>>();
                using (var connection = new NpgsqlConnection(connString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM patients", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        using (var reader = cmd.ExecuteReader())
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
                }

                string json = JsonConvert.SerializeObject(results, Formatting.Indented);
                return json;
            }
            else
            {
                throw new UnauthorizedAccessException("Authentication Failed");
            }
        }

        public void InsertPatient(Patient p, string authId)
        {
            var secureUserData = new SecureUserData();
            if (secureUserData.IsSecure(authId))
            {
                var connString = _configuration.GetConnectionString("bizlabcoreapiContext");
                using (var connection = new NpgsqlConnection(connString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(PatientData.InsertPatientSql, connection))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@id", p.Id);
                        cmd.Parameters.AddWithValue("@client_id", (object?)p.ClientId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@legacy_id", (object?)p.LegacyId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@first_name", (object?)p.FirstName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@last_name", (object?)p.LastName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@name", (object?)p.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone", (object?)p.Phone ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@address_street", (object?)p.AddressStreet ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@address_city", (object?)p.AddressCity ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@address_state", (object?)p.AddressState ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@address_zip", (object?)p.AddressZip ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@patient_status", (object?)p.PatientStatus ?? "active");
                        cmd.Parameters.AddWithValue("@account_status", (object?)p.AccountStatus ?? "active");

                        cmd.Parameters.AddWithValue("@nickname", (object?)p.Nickname ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@title", (object?)p.Title ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@birth_date", (object?)p.BirthDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@company", (object?)p.Company ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@email_opt_in", p.EmailOptIn);
                        cmd.Parameters.AddWithValue("@sms_opt_in", p.SmsOptIn);
                        cmd.Parameters.AddWithValue("@memo", (object?)p.Memo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@cancellation_list", p.CancellationList);
                        cmd.Parameters.AddWithValue("@first_clinical_service_complete", p.FirstClinicalServiceComplete);
                        cmd.Parameters.AddWithValue("@first_clinical_service_date", (object?)p.FirstClinicalServiceDate ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@membership_status", (object?)p.MembershipStatus ?? "none");
                        cmd.Parameters.AddWithValue("@membership_start_date", (object?)p.MembershipStartDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@membership_type", (object?)p.MembershipType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone_work", (object?)p.PhoneWork ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@phone_mobile", (object?)p.PhoneMobile ?? DBNull.Value);

                        //cmd.Parameters.AddWithValue("@resources", p.Resources != null
                        //    ? Newtonsoft.Json.JsonConvert.SerializeObject(p.Resources)
                        //    : "[]");

                        cmd.Parameters.AddWithValue("@notes", (object?)p.Notes ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@created_at", p.CreatedAt);
                        cmd.Parameters.AddWithValue("@updated_at", p.UpdatedAt);

                        cmd.ExecuteNonQuery();
                    }
                    ;
                }
            }
            else {
                throw new UnauthorizedAccessException("Authentication Failed");
            }
        }

        public string InsertMasterPatient(MasterPatient patientMaster)
        {
            //JObject patientMaster = JObject.Parse(inputData);
            const string sql = @"
        INSERT INTO master_patients (
            id,
            patient_id,
            client_id,
            first_name,
            last_name,
            email,
            phone,
            date_of_birth,
            address,
            city,
            state,
            zip_code,
            created_at,
            updated_at
        )
        VALUES (
            @id,
            @patient_id,
            @client_id,
            @first_name,
            @last_name,
            @email,
            @phone,
            @date_of_birth,
            @address,
            @city,
            @state,
            @zip_code,
            NOW(),
            NOW()
        );
    ";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("id", new Guid(patientMaster.id));
            cmd.Parameters.AddWithValue("patient_id", patientMaster.patient_id);
            cmd.Parameters.AddWithValue("client_id", patientMaster.client_id);
            cmd.Parameters.AddWithValue("first_name", patientMaster.first_name ?? "");
            cmd.Parameters.AddWithValue("last_name", patientMaster.last_name ?? "");
            cmd.Parameters.AddWithValue("email", patientMaster.email ?? "");
            cmd.Parameters.AddWithValue("phone", patientMaster.phone ?? "");
            cmd.Parameters.AddWithValue("date_of_birth", DateTime.Parse(patientMaster.date_of_birth) != null ? DateTime.Parse(patientMaster.date_of_birth) : null);
            cmd.Parameters.AddWithValue("address", patientMaster.address ?? "");
            cmd.Parameters.AddWithValue("city", patientMaster.city ?? "");
            cmd.Parameters.AddWithValue("state", patientMaster.state ?? "");
            cmd.Parameters.AddWithValue("zip_code", patientMaster.zip_code ?? "");

            cmd.ExecuteNonQuery();

            return patientMaster.id;
        }

      
    }
}
