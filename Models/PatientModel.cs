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

}
