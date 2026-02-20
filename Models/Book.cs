using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }

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