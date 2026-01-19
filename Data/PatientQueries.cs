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
                    using (var cmd = new NpgsqlCommand("DELETE FROM master_patients where id='" + id + "'", connection))
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
                using (var cmd = new NpgsqlCommand("UPDATE master_patients set patient_status='inactive' where id='" + id + "'", connection))
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
                    using (var cmd = new NpgsqlCommand("SELECT * FROM master_patients", connection))
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

        public void UpdatePatientData(PatientUpdateData patient, string authId)
        {
            string sql = @"
            UPDATE master_patients
            SET
                first_name = COALESCE(@first_name, first_name),
                last_name = COALESCE(@last_name, last_name),
                email = COALESCE(@email, email),
                phone = COALESCE(@phone, phone),
                address = COALESCE(@address, address),
                city = COALESCE(@city, city),
                state = COALESCE(@state, state),
                zip_code = COALESCE(@zip_code, zip_code),
                patient_type = COALESCE(@patient_type, patient_type),
                date_of_birth = @date_of_birth,
                gender = COALESCE(@gender, gender)
            WHERE id = @id;
            ";

            var secureUserData = new SecureUserData();
            if (secureUserData.IsSecure(authId))
            {
                using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
                conn.Open();

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", patient.Id);
                cmd.Parameters.AddWithValue("@first_name", (object?)patient.first_name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@last_name", (object?)patient.last_name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object?)patient.email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", (object?)patient.phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@address", (object?)patient.address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@city", (object?)patient.city ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@state", (object?)patient.state ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@zip_code", (object?)patient.zip_code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@patient_type", (object?)patient.patient_status ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@date_of_birth", patient.date_of_birth != "" ? DateTime.Parse(patient.date_of_birth).ToShortDateString() : null);
                cmd.Parameters.Add(
                                    new NpgsqlParameter("@date_of_birth", NpgsqlTypes.NpgsqlDbType.Date)
                                    {
                                        Value = patient.date_of_birth != "" ? DateTime.Parse(patient.date_of_birth) : (object?)DBNull.Value
                                    });
                cmd.Parameters.AddWithValue("@gender", (object?)patient.gender ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@emergency_relationship", (object?)patient.emergency_relationship ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@emergency_contact_phone", (object?)patient.emergency_contact_phone ?? DBNull.Value);
                //cmd.Parameters.AddWithValue("@emergency_name", (object?)patient.emergency_name ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
            else
            {
                throw new UnauthorizedAccessException("Authentication Failed");
            }
        }

        public string InsertData(dynamic data)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            dynamic patientMasters = JsonConvert.DeserializeObject<dynamic>(data);
            var sql = @"
INSERT INTO master_patients (
    id,
    patient_id,
    first_name,
    last_name,
    phone,
    email,
    address,
    city,
    state,
    zip_code,
    patient_type,
    first_visit_date,
    first_visit_location,
    first_visit_service,
    date_added,
    last_contact_date,
    last_visit_date,
    status,
    status_reason,
    status_changed_date,
    referral_source,
    marketing_consent,
    email_opt_in,
    sms_opt_in,
    created_at,
    updated_at,
    created_by,
    updated_by,
    submission_id,
    dob,
    preferred_location,
    tt_enrollment_status,
    tt_current_week,
    tt_program_id,
    tt_start_date,
    tt_completion_date,
    date_of_birth,
    gender,
    client_id,
    signature_circle_id,
    signature_circle_status,
    signature_circle_joined_at,
    signature_circle_referrer_id,
    signature_circle_qr_code,
    signature_circle_referral_count,
    signature_circle_total_earned,
    signature_circle_auto_reload,
    signature_circle_auto_reload_amount,
    signature_circle_auto_reload_threshold,
    is_active,
    notes,
    call_status,
    call_notes,
    next_appointment_date,
    family_group_id
) VALUES (
    @id,
    @patient_id,
    @first_name,
    @last_name,
    @phone,
    @email,
    @address,
    @city,
    @state,
    @zip_code,
    @patient_type,
    @first_visit_date,
    @first_visit_location,
    @first_visit_service,
    @date_added,
    @last_contact_date,
    @last_visit_date,
    @status,
    @status_reason,
    @status_changed_date,
    @referral_source,
    @marketing_consent,
    @email_opt_in,
    @sms_opt_in,
    @created_at,
    @updated_at,
    @created_by,
    @updated_by,
    @submission_id,
    @dob,
    @preferred_location,
    @tt_enrollment_status,
    @tt_current_week,
    @tt_program_id,
    @tt_start_date,
    @tt_completion_date,
    @date_of_birth,
    @gender,
    @client_id,
    @signature_circle_id,
    @signature_circle_status,
    @signature_circle_joined_at,
    @signature_circle_referrer_id,
    @signature_circle_qr_code,
    @signature_circle_referral_count,
    @signature_circle_total_earned,
    @signature_circle_auto_reload,
    @signature_circle_auto_reload_amount,
    @signature_circle_auto_reload_threshold,
    @is_active,
    @notes,
    @call_status,
    @call_notes,
    @next_appointment_date,
    @family_group_id
);
";

            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("bizlabcoreapiContext"));
            conn.Open();
            using var cmdDelete = new NpgsqlCommand("delete from master_patients", conn);
            cmdDelete.ExecuteNonQuery();
            int updatedRecs = 0;
            for (int i = 0; i < patientMasters.Count; i ++)
            {
                var patientMaster = patientMasters[i];
                //using var cmdDelete = new NpgsqlCommand("select count(1) from master_patients where id='" + patientMaster.id.Value + "'", conn);
                //var recsData = cmdDelete.ExecuteScalar().ToString();

                if(updatedRecs >= 0)
                {
                    using var cmd = new NpgsqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("id", Guid.Parse(patientMaster.id.Value));
                    cmd.Parameters.AddWithValue("patient_id", patientMaster.patient_id.Value);
                    cmd.Parameters.AddWithValue("first_name", patientMaster.first_name.Value != null ? patientMaster.first_name.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("last_name", patientMaster.last_name.Value != null ? patientMaster.last_name.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("phone", patientMaster.phone.Value != null ? patientMaster.phone.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("email", patientMaster.email.Value != null ? patientMaster.email.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("address", patientMaster.address.Value != null ? patientMaster.address.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("city", patientMaster.city.Value != null ? patientMaster.city.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("state", patientMaster.state.Value != null ? patientMaster.state.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("zip_code", patientMaster.zip_code.Value != null ? patientMaster.zip_code.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("patient_type", patientMaster.patient_type.Value != null ? patientMaster.patient_type.Value : DBNull.Value);

                    cmd.Parameters.AddWithValue("first_visit_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("first_visit_location", DBNull.Value);
                    cmd.Parameters.AddWithValue("first_visit_service", DBNull.Value);

                    cmd.Parameters.AddWithValue("date_added", patientMaster.date_added.Value != null ? DateTimeOffset.Parse(patientMaster.date_added.Value.ToString()) : DBNull.Value);
                    cmd.Parameters.AddWithValue("last_contact_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("last_visit_date", DBNull.Value);

                    cmd.Parameters.AddWithValue("status", patientMaster.status.Value != null ? patientMaster.status.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("status_reason", DBNull.Value);
                    cmd.Parameters.AddWithValue("status_changed_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("referral_source", DBNull.Value);

                    cmd.Parameters.AddWithValue("marketing_consent", false);
                    cmd.Parameters.AddWithValue("email_opt_in", true);
                    cmd.Parameters.AddWithValue("sms_opt_in", true);

                    cmd.Parameters.AddWithValue("created_at", DateTimeOffset.Parse(patientMaster.created_at.Value.ToString()));
                    cmd.Parameters.AddWithValue("updated_at", DateTimeOffset.Parse(patientMaster.updated_at.Value.ToString()));

                    cmd.Parameters.AddWithValue("created_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("updated_by", DBNull.Value);
                    cmd.Parameters.AddWithValue("submission_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("dob", DBNull.Value);
                    cmd.Parameters.AddWithValue("preferred_location", DBNull.Value);
                    cmd.Parameters.AddWithValue("tt_enrollment_status", DBNull.Value);

                    cmd.Parameters.AddWithValue("tt_current_week", 0);
                    cmd.Parameters.AddWithValue("tt_program_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("tt_start_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("tt_completion_date", DBNull.Value);

                    cmd.Parameters.AddWithValue("date_of_birth", patientMaster.date_of_birth.Value != null ? DateTime.Parse(patientMaster.date_of_birth.Value.ToString()) : DBNull.Value);
                    cmd.Parameters.AddWithValue("gender", DBNull.Value);

                    cmd.Parameters.AddWithValue("client_id", patientMaster.client_id.Value);

                    cmd.Parameters.AddWithValue("signature_circle_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("signature_circle_status", patientMaster.signature_circle_status.Value != null ? patientMaster.signature_circle_status.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("signature_circle_joined_at", DBNull.Value);
                    cmd.Parameters.AddWithValue("signature_circle_referrer_id", DBNull.Value);
                    cmd.Parameters.AddWithValue("signature_circle_qr_code", DBNull.Value);

                    cmd.Parameters.AddWithValue("signature_circle_referral_count", 0);
                    cmd.Parameters.AddWithValue("signature_circle_total_earned", 0.00m);
                    cmd.Parameters.AddWithValue("signature_circle_auto_reload", false);
                    cmd.Parameters.AddWithValue("signature_circle_auto_reload_amount", 25.00m);
                    cmd.Parameters.AddWithValue("signature_circle_auto_reload_threshold", 25.00m);

                    cmd.Parameters.AddWithValue("is_active", true);
                    cmd.Parameters.AddWithValue("notes", DBNull.Value);
                    cmd.Parameters.AddWithValue("call_status", patientMaster.call_status.Value != null ? patientMaster.call_status.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("call_notes", DBNull.Value);
                    cmd.Parameters.AddWithValue("next_appointment_date", DBNull.Value);
                    cmd.Parameters.AddWithValue("family_group_id", DBNull.Value);

                    cmd.ExecuteNonQuery();
                    updatedRecs++;
                }
            }

            return "Supabase Records : " + patientMasters.Count.ToString() + " ## GCP Updated Recs : " + updatedRecs.ToString();
        }
    }
}
