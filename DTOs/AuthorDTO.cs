using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}