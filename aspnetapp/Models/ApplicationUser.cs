using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace assignment_4.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    [Required, StringLength(64, MinimumLength = 5)]
    public string Nickname { get; set; } = String.Empty;
}