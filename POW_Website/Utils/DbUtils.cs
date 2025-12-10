using Microsoft.EntityFrameworkCore;
using POWStudio.Models;

namespace POWStudio.Utils;

public class DbUtils
{
    // public static void CreateDatabase()
    // {
    //     using var dbCtx = new GameStoreDbContext();
    //     string DbName = dbCtx.Database.GetDbConnection().Database;
    //     var result = dbCtx.Database.EnsureCreated();
    //     
    //     Console.WriteLine(DbName + " Create :" + (result ? " Success" : "Failed"));
    // }
    // public static void DeleteDropDatabase()
    // {
    //     using var dbCtx = new GameStoreDbContext();
    //     string DbName = dbCtx.Database.GetDbConnection().Database;
    //     var result = dbCtx.Database.EnsureDeleted();
    //     
    //     Console.WriteLine(DbName + " Deleled :" + (result ? " Success" : "Failed"));
    // }

    public static void InsertModel<T>(T inModel)
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.Add(inModel);
        dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + inModel.ToString());
    }

    public static int GetWishlistId(string inUserId)
    {
        using var dbCtx = new GameStoreDbContext();
        var wishlist = dbCtx.Wishlist.FirstOrDefault(w => w.UserId == inUserId);
        if (wishlist != null) return wishlist.Id;
        wishlist = new Wishlist
        {
            UserId = inUserId
        };
        
        dbCtx.Wishlist.Add(wishlist);
        dbCtx.SaveChanges();

        return wishlist.Id;
    }
    public static int GetCartId(string inUserId)
    {
        using var dbCtx = new GameStoreDbContext();
        var cart = dbCtx.Cart.FirstOrDefault(c => c.UserId == inUserId);
        if (cart != null) return cart.Id;
        cart = new Cart
        {
            UserId = inUserId
        };
        
        dbCtx.Cart.Add(cart);
        dbCtx.SaveChanges();

        return cart.Id;
    }
    public static bool IsGameInWishlist(int gameId, int wishlistId)
    {
        using var dbCtx = new GameStoreDbContext();
        return dbCtx.WishlistItem.Any(wi=>wi.GameId == gameId && wi.WishlistId == wishlistId);
    }
    public static bool IsGameInCart(int inGameId, int cartId)
    {
        using var dbCtx = new GameStoreDbContext();
        return dbCtx.CartItem.Any(ci=>ci.GameId == inGameId && ci.CartId == cartId);
    }
    public static int GetLibraryId(string inUserId)
    {
        using var dbCtx = new GameStoreDbContext();
        var library = dbCtx.UserLibrary.FirstOrDefault(l =>l.UserId == inUserId);
        if (library != null) return library.Id;
        library = new UserLibrary()
        {
            UserId = inUserId
        };
        
        dbCtx.UserLibrary.Add(library);
        dbCtx.SaveChanges();

        return library.Id;
    }
    public static bool IsGameInLibrary(int inGameId, int libraryId)
    {
        using var dbCtx = new GameStoreDbContext();
        return dbCtx.LibraryItem.Any(l=>l.GameId == inGameId && l.LibraryId == libraryId);
    }
    //Hardcode to insert into db_____________________________________
    public static void HardcodeInsertGameScreenshot()
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.AddRange(gameScreenshotList);
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + "game screenshot");
    }
    public static void ChangeGameReleasedDateHardCode()
    {
        using var dbCtx = new GameStoreDbContext();
        var gameList = dbCtx.Game.ToList();
        foreach (var game in gameList) game.ReleaseDate = DateTime.Now;
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + "game ReleasedDate");
    }
    public static void HardCodeInsertGameCate()
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.AddRange(gameCateList);
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + "game cate");
    }
    public static void HardCodeInsertGame()
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.AddRange(gameList);
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + "game data");
    }
    public static void HardCodeInsertCategory()
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.AddRange(categoryList);
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + " category data");
    }

    public static void HardcodeInsertRating()
    {
        using var dbCtx = new GameStoreDbContext();
        dbCtx.AddRange(gameCateList);
        int number_rows = dbCtx.SaveChanges();
        Console.WriteLine("Inserted " + number_rows + "rating data");
    }
    //hard code data
    // private static List<Rate> gameRatingList =
    // [
    //     new Rate
    //     {
    //         bRecomended = true,
    //         GameId = 2,
    //         UserId = "d019fc0b-e2ba-4a97-985a-3c8646456e57",
    //         Date = DateTime.Now,
    //     }
    // ];
    private static List<GameScreenshot> gameScreenshotList =
    [
        new GameScreenshot {GameId = 1, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY1MDY0MC8yMTcyMzU4Ni5qcGc=/original/uBTTFI.jpg"},
        new GameScreenshot {GameId = 1, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY1MDY0MC8yMTcyOTA5Ny5wbmc=/347x500/vgeJiL.png"},
        new GameScreenshot {GameId = 1, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY1MDY0MC8yMTcyOTEyMC5qcGc=/original/j3SMAV.jpg"},
        
        new GameScreenshot {GameId = 2, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzkyNzM0NC8yMzUyOTMxNS5qcGc=/original/sTyw65.jpg"},
        new GameScreenshot {GameId = 2, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzkyNzM0NC8yMzUyOTMxNi5qcGc=/original/Di1Ffj.jpg"},
        new GameScreenshot {GameId = 2, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzkyNzM0NC8yNDA5OTg4NC5qcGc=/original/KhXqgQ.jpg"},
        new GameScreenshot {GameId = 2, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzkyNzM0NC8yNDA5OTg3My5qcGc=/original/kXCf6I.jpg"},
        
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzMxMjIyNS5qcGc=/original/HLQtIG.jpg"},
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzM4Mzk4OC5qcGc=/original/oBE%2F1v.jpg"},
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzM4Mzk4Ni5qcGc=/original/425yjX.jpg"},
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzM4Mzk5MC5qcGc=/original/L9jsuN.jpg"},
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzM4Mzk4OS5qcGc=/original/QRqq2F.jpg"},
        new GameScreenshot {GameId = 3, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjE3OTE1NC8xMzM4MzkxNC5qcGc=/original/yOb98J.jpg"},
        
        new GameScreenshot {GameId = 4, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjEzMjIzNS8xMjU2NTYzNS5qcGc=/original/UuI0Oo.jpg"},
        new GameScreenshot {GameId = 4, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjEzMjIzNS8xMjU2NTYxNC5qcGc=/original/v7JQyb.jpg"},
        new GameScreenshot {GameId = 4, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjEzMjIzNS8xMjU2NTYxOS5qcGc=/original/S0wk3a.jpg"},
        new GameScreenshot {GameId = 4, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjEzMjIzNS8xMjU2NTYyNi5qcGc=/original/Qd8nIh.jpg"},
        new GameScreenshot {GameId = 4, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMjEzMjIzNS8xMjU4NTE5NS5qcGc=/original/fbaY8c.jpg"},
        
        new GameScreenshot {GameId = 5, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY4NTA3Ni8yMTkyNjU1NC5qcGc=/original/TnucwP.jpg"},
        new GameScreenshot {GameId = 5, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY4NTA3Ni8yMTkyNjU1NS5qcGc=/original/ShFruB.jpg"},
        new GameScreenshot {GameId = 5, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzY4NTA3Ni8yMTkyNjU2MC5qcGc=/original/mhO%2F%2Fe.jpg"},
        
        new GameScreenshot {GameId = 6, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzQ3MjEyNi8yMDcxNTQ1NS5qcGc=/original/t%2BppUG.jpg"},
        new GameScreenshot {GameId = 6, ScreenshotUrl = "https://img.itch.zone/aW1hZ2UvMzQ3MjEyNi8yMDcxNTI4Ny5qcGc=/original/OTYTLV.jpg"}
        
    ];
    private static List<GameCategory> gameCateList =
    [
        new GameCategory { CategoryId = 2, GameId = 2 },
        new GameCategory { CategoryId = 1, GameId = 3 },
        new GameCategory { CategoryId = 3, GameId = 4 },
        new GameCategory { CategoryId = 3, GameId = 5 },
        new GameCategory { CategoryId = 1, GameId = 6 }
    ];
    private static List<Category> categoryList =
    [
        new Category { Name = "Action" },
        new Category { Name = "Adventure" },
        new Category { Name = "Horror" },
        new Category { Name = "Puzzle" },
    ];
    private static List<Game?> gameList =
    [
        new Game
        {
            bPublic = false,
            Title = "Bedoso Monster",
            Slug = "bedoso-monster",
            ShortDescription = "Where peace once lived, monsters now rule",
            DetailedDescription =
                "Enter Bedoso Monster, a tense third-person shooter where you will face the nameless horrors lurking in the deserted town of Bedoso. After a mysterious event, this once peaceful town is now plunged into darkness and taken over by hideous monsters of unknown origin.",
            TitleImageUrl = "/Image/Logo/BedosoMonsterTitleWhite.png",
            BgImageUrl = "/Image/ArtworkGame/BedosoMonsterBG.jpg",
            SteamUrl = "https://store.steampowered.com/app/bedoso-monster",
            ItchioUrl = "https://powstudio.itch.io/bedoso-monster",
            EpicUrl = "https://store.epicgames.com/en-US/p/bedoso-monster",
            TrailerUrl = "https://www.youtube.com/embed/hHyBYI8EF-c?si=WrwJn6gHF1ZI4MUF",
        },

        new Game
        {
            Title = "Pictureal",
            Slug = "pictureal",
            TitleImageUrl = "/Image/Logo/PicturealGameTittle.png",
            DetailedDescription =
                "Tựa game giải đố phiêu lưu góc nhìn thứ nhất. Trải nhiệm câu chuyện sử dụng các điển tích trong tôn giáo, cùng với cơ chế thú vị chụp ảnh, đặt lại vật thể trong ảnh ra môi  trường theo đúng góc nhìn của ảnh",
            ShortDescription = "Phiêu lưu trong thế giới siêu thực",
            BgImageUrl = "/Image/ArtworkGame/PicturealArtwork.jpg",
            ItchioUrl = "https://powstudio.itch.io/pictureal",
            TrailerUrl = "https://www.youtube.com/embed/hMAkXnCutAw?si=d7tB9yqtC-7q1TEX"
        },

        new Game
        {
            Title = "Horror Serum",
            Slug = "horrorserum",
            TitleImageUrl = "/Image/Logo/HorrorSerumTitle.png",
            ShortDescription = "It promised power. It delivered torment.",
            BgImageUrl = "/Image/ArtworkGame/HorrorSerumArtwork.jpg",
            ItchioUrl = "https://powstudio.itch.io/horrorserum",
            DetailedDescription =
                "Horror Serum, you will experience the horror from a first-person perspective, discovering a dark story surrounding a mysterious serum. This solution promises extraordinary power, but the price to pay may be beyond imagination.",
            TrailerUrl = "https://www.youtube.com/embed/y8PKcCArLQ0?si=AyBEMqYDRoU6cIVM"
        },

        new Game
        {
            Title = "Bad Of Greenthorn",
            Slug = "bad-of-greenthorn",
            TitleImageUrl = "/Image/Logo/BadOfGreenThornTitleWhite.png",
            ShortDescription = "A fallen mage. A cursed land. One last hope.", //Strike down darkness. Save the kingdom.
            BgImageUrl = "/Image/ArtworkGame/BadOfGreenThorn capture.jpg",
            ItchioUrl = "https://powstudio.itch.io/bad-of-greenthorn",
            DetailedDescription =
                "Bad of Greenthorn is a 2D hack and slash action platformer that takes you on a thrilling adventure! As the hero of the kingdom, you will embark on a dangerous journey to destroy the treacherous mage Greenthorn - who has spread chaos and darkness everywhere.",
            TrailerUrl = "https://www.youtube.com/embed/UOMMnQUUchM?si=PcooWto9nK3Z7dLe"
        },


        new Game
        {
            Title = "HIT REACTION ANIMATIONS",
            Slug = "hit-reaction-animations",
            ShortDescription = "Enemy hit reaction animations – minimal, usable, effective.",
            DetailedDescription =
                "This pack includes carefully crafted animations for enemy hit reactions, designed to fit real gameplay scenarios. Instead of focusing on quantity, this Asset is designed with the goal of optimizing costs for real game projects - no excess, no frills, but enough for most gameplay situations. Ideal for monster, zombie games, or any project that needs grounded and responsive enemy reactions.",
            BgImageUrl = "/Image/ArtworkGame/HitReactionAnimationsBg.jpg",
            ItchioUrl = "https://powstudio.itch.io/hit-reaction-animations",
            FabUrl = "https://fab.com/s/bf12ffe297f2",
            TrailerUrl = "https://www.youtube.com/embed/RrXuBIbXjQE?si=we2LmLwvO4vV-tBw"
        },

        new Game
        {
            Title = "Interactive Paper",
            Slug = "interactive-paper",
            ShortDescription = "Tool for interactive paper game.",
            BgImageUrl = "/Image/ArtworkGame/PaperInteractive16div9.png",
            ItchioUrl = "https://powstudio.itch.io/interactive-paper-effect-ue",
            FabUrl = "https://fab.com/s/96b6838c5448",
            TrailerUrl = "https://www.youtube.com/embed/aoogGi-WoGo?si=IsxShWRyt3uTv8Kb"
        }
    ];
}