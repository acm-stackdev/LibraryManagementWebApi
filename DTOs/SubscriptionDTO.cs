using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        
        [Required]
        public string UserId { get; set; }

        [Required]
       public int NumberofMonths { get; set; }
    }
}