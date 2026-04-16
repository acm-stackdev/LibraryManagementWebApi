using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class UpdateBookDTO
    {

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string ISBN { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int PublishedYear { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public int TotalPages { get; set; }

        [Required]
        public List<string> AuthorNames { get; set; }
    }
}