using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class AuthModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; };
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; };
}
