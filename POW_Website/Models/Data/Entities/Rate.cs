using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class Rate
{
    public int Id { get; set; }
    public bool bRecomended { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
    public int? LikeAmount { get; set; }
    public int? DislikeAmount { get; set; }
    public int? FunnyAmount { get; set; }
    
    public int GameId { get; set; }
    [StringLength(450)]
    public required string UserId { get; set; }
    
    //Navigation properties for FK
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}