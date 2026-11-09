
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

/// <summary>
/// Holds the information entered on the MVC login page.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

/// <summary>
/// Holds the information entered when a new user registers.
/// </summary>
public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    [Required]
    public string Role { get; set; } = "Participant";
}

/// <summary>
/// Matches the successful login response returned by the Part 2 API.
/// </summary>
public class ApiLoginResponse
{
    public string? Message { get; set; }
    public string? Token { get; set; }
    public int UserID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// Matches the user details returned after successful registration.
/// </summary>
public class ApiRegisterResponse
{
    public int UserID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
}
