using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int BorrowRecordId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [JsonIgnore]
        public AppUser? User { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }
    }
}