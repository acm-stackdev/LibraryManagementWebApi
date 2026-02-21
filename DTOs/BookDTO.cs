using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class BookDTO
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
    }
}