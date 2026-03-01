using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class BorrowRecordDTO
    {
        public int BorrowRecordId { get; set; }
        
        public string UserId { get; set; }

        public string UserName { get; set; }
       
        [Required]
        public int BookId { get; set; }
        
        public string BookTitle { get; set; }
        
        public DateTime BorrowDate { get; set; }
        
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}