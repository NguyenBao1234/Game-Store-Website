using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int GameId { get; set; }
    
    //Navigation properties for FK
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
    
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
}