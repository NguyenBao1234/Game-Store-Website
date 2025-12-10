using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class LibraryItem
{
    [Key]
    public int Id { get; set; }
    
    public int LibraryId { get; set; }
    
    public int GameId { get; set; }
    
    //Navigation properties for FK
    [ForeignKey("LibraryId")]
    public UserLibrary? UserLibrary { get; set; }
    
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
}