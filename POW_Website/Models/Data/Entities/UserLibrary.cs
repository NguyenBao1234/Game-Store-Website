using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class UserLibrary
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(450)]
    public required string UserId { get; set; }
    
    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}