using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<BookAuthor>? BookAuthors { get; set; }
    }
}