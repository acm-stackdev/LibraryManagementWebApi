using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Book>? Books { get; set; }
    }
}