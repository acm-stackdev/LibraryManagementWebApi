using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public String UserId { get; set; }

        public int MaxBorrowLimit { get; set; }
        public int BorrowDurationDays { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [JsonIgnore]
        public AppUser? User { get; set; }
    }
}