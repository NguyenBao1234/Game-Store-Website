using System.ComponentModel.DataAnnotations.Schema;

namespace POWStudio.Models;

public class GameScreenshot
{
    public int Id {get; set;}
    public int GameId { get; set; }
    public string? ScreenshotUrl { get; set; }
    //Navigation properties for FK
    [ForeignKey("GameId")]
    public Game? Game { get; set; }
}