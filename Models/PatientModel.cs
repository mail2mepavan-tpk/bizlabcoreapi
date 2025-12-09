using System;
using System.Collections.Generic;

namespace bizlabcoreapi.Models
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string? ClientId { get; set; }
        public string? LegacyId { get; set; }

        // Basic Info
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Address
        public string? AddressStreet { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressState { get; set; }
        public string? AddressZip { get; set; }

        // Status
        public string? PatientStatus { get; set; }
        public string? AccountStatus { get; set; }

        // New Fields
        public string? Nickname { get; set; }
        public string? Title { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Company { get; set; }
        public bool EmailOptIn { get; set; }
        public bool SmsOptIn { get; set; }
        public string? Memo { get; set; }
        public bool CancellationList { get; set; }
        public bool FirstClinicalServiceComplete { get; set; }
        public DateTime? FirstClinicalServiceDate { get; set; }
        public string? MembershipStatus { get; set; }
        public DateTime? MembershipStartDate { get; set; }
        public string? MembershipType { get; set; }
        public string? PhoneWork { get; set; }
        public string? PhoneMobile { get; set; }

        // JSONB (resources)
        public List<object>? Resources { get; set; }

        public string? Notes { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MasterPatient()
    {
        public string id { get; set; }
        public string patient_id { get; set; }
        public string client_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string date_of_birth { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}
