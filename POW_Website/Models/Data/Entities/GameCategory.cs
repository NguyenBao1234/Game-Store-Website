using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class GameCategory
{
    [Key]
    public int Id { get; set; }
    
    public int GameId { get; set; }
    public int CategoryId { get; set; }
    
    //Navigation properties for FK
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}