using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(450)]
    public required string UserId { get; set; }
    
    
    [ForeignKey("UserId")]
    public required ApplicationUser User { get; set; }
}