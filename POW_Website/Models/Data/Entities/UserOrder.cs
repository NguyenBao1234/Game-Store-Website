using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace POWStudio.Models;

public class UserOrder
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(450)]
    public required string UserId { get; set; }
    
    
    [Required]
    [ForeignKey("UserId")]
    public required IdentityUser User { get; set; }
}