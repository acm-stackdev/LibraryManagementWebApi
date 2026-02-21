using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class BorrowRecordDTO
    {
        public int BorrowRecordId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public DateTime BorrowDate { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}