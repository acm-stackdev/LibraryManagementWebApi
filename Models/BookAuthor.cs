using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class BookAuthor
    {
        public int BookAuthorId { get; set; }
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }

        [JsonIgnore]
        public Author? Author { get; set; }
    }
}