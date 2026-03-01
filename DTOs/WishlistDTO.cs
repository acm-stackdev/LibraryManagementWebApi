using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystem.DTOs
{
    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public List<AuthorDTO> Authors { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}