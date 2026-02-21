using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }

        [JsonIgnore]
        public List<BookAuthor>? BookAuthors { get; set; }

        [JsonIgnore]
        public List<BorrowRecord>? BorrowRecords { get; set; }

        [JsonIgnore]
        public List<Wishlist>? Wishlists { get; set; }
    }
}