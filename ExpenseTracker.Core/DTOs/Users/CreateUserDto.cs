
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Core.DTOs.Users;

public class CreateUserDto
{
    [Required]
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}
