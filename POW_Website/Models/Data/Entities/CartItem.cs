using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    
    public int CartId { get; set; }
    
    public int GameId { get; set; }
    
    //Navigation properties for FK
    [ForeignKey("OrderId")]
    public Cart? Cart { get; set; }
    
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
}