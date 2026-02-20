using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord
    {
        public int BorrowRecordId { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [JsonIgnore]
        public AppUser? User { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }
    }
}