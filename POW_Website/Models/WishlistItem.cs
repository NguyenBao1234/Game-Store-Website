using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class WishlistItem
{
    //SimpleKey
    public int Id { get; set; }
    
    public int WishlistId { get; set; }
    
    public int GameId { get; set; }

    [ForeignKey("WishlistId")]
    public Wishlist Wishlist { get; set; }
    
    [ForeignKey("GameId")]
    public Game Game { get; set; }
}