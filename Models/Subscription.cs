using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        
        [Required]
        public String UserId { get; set; }

        [Required]
        public int MaxBorrowLimit { get; set; }
        
        [Required]
        public int BorrowDurationDays { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
        
        [JsonIgnore]
        public AppUser? User { get; set; }
    }
}