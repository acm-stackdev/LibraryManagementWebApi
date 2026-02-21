using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}