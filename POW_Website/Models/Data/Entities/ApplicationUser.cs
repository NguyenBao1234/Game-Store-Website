using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace POWStudio.Models;

public class ApplicationUser :  IdentityUser
{
    [StringLength(50)]
    public string? DisplayName { get; set; }
}