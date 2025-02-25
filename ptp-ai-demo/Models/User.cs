using System;
using System.ComponentModel.DataAnnotations;

namespace ptp_ai_demo.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username must be under 100 characters.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password hash is required.")]
    public string PasswordHash { get; set; }
    
    public string Role { get; set; }
}