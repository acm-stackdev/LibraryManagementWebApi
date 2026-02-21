using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        public int AuthorId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [JsonIgnore]
        public List<BookAuthor>? BookAuthors { get; set; }
    }
}