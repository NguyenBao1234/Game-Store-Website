using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace POWStudio.Models;

public class Game
{
    public int Id { get; set; }// key naming convention auto
    [Column(TypeName = "ntext")]
    public string? Title { get; set; }
    public string? Slug { get; set; } = string.Empty;
    public string? TitleImageUrl { get; set; } = string.Empty;
    public string? BgImageUrl { get; set; } = string.Empty;
    public string? ShortDescription { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; } = string.Empty;
    public bool bPublic { get; set; } = true;
    public string? TrailerUrl { get; set; }
    public string? ItchioUrl { get; set; } = string.Empty;
    public string? SteamUrl { get; set; }
    public string? EpicUrl { get; set; }
    public string? FabUrl { get; set; }
    [Column(TypeName = "money")]
    public decimal? Price { get; set; }
    public float? DiscountPercent { get; set; }
    public DateTime? ReleaseDate { get; set; }
}
//[Table("TableName")],
//[Key] -> Primary Key (PK)
//[Required] -> not null
//[StringLength(50)] -> string - nvarchar
//[Column("Tensanpham", TypeName = "ntext")]