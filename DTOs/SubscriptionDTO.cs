using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int MaxBorrowLimit { get; set; }

        [Required]
        public int BorrowDurationDays { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}