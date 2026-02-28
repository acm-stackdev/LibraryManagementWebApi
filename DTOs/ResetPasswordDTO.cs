using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs{

    public class ResetPasswordDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}