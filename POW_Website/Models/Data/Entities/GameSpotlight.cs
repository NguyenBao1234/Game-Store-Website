using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class GameSpotlight
{
    [Key]
    public int GameId { get; set; }
    
    
    public int Priority {get; set;}
    
    
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
}