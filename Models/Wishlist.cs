using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class Wishlist
    {
        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public AppUser? User { get; set; }
        
        [JsonIgnore]
        public Book? Book { get; set; }
        
    }
}