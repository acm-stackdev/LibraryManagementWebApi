using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class RegisterModel
{
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}
