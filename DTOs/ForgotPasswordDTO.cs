using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}