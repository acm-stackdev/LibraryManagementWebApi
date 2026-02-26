using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        [JsonIgnore]
        public Subscription? Subscription { get; set; }

        [JsonIgnore]
        public List<BorrowRecord>? BorrowRecords { get; set; }

        [JsonIgnore]
        public List<Wishlist>? Wishlists { get; set; }
    }
}