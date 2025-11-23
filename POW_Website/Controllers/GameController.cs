using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Models.Enum;
using POWStudio.Services;
using POWStudio.Utils;

namespace POWStudio.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;
    private readonly GameStoreDbContext mDbContext;

    public GameController(IGameService gameService, GameStoreDbContext mDbContextContext)
    {
        _gameService = gameService;
        mDbContext = mDbContextContext;
    }

    [Route("/Games")]
    [HttpGet]
    public IActionResult Index(string inSearchTerm, List<string> inCategoryNames, decimal? inMinPrice, decimal? inMaxPrice, GameSortOption inSortOption, bool inSortAscending = true)
    {
        IQueryable<Game> games;

        var categoryObjs = _gameService.GetAllCategories().ToList();
        List<string> categories = new List<string>();
        foreach (var category in categoryObjs) categories.Add(category.Name);
        
        List<int>SelectCategoryIds = new List<int>();
        foreach (var category in categoryObjs)
            if (inCategoryNames.Contains(category.Name))  SelectCategoryIds.Add(category.Id);
        
        ViewData["AllCategories"] = categories;
        
        if (string.IsNullOrEmpty(inSearchTerm)) games = _gameService.GetAll();
        else games = _gameService.GetGamesByTerm(inSearchTerm, 0, true);
        var sortedGames = _gameService.GetGamesBySortOption(games, inSortOption, inSortAscending);
        var priceRangeGames = _gameService.GetGamesByPriceRange(sortedGames, inMinPrice??0, inMaxPrice??999999);
        var gameByFilter = _gameService.GetGamesByCategory(priceRangeGames,SelectCategoryIds);

        ViewData["CurrentSearchTerm"] = inSearchTerm;
        ViewData["SelectedCategories"] =  inCategoryNames;
        ViewData["SortOption"] = inSortOption;
        ViewData["SortAscending"] = inSortAscending;
        ViewData["MinPrice"] = inMinPrice;
        ViewData["MaxPrice"] = inMaxPrice;
        return View(gameByFilter.ToList());
    }
    
    [HttpGet]
    public IActionResult SuggestGames(string term)
    {
        if (string.IsNullOrEmpty(term)) return Content("");
        
        var gameSuggestions = _gameService.GetGamesByTerm(term, 4, false); 
        
        // Truyền từ khóa tìm kiếm để dùng cho việc highlight trong View
        ViewData["SearchTerm"] = term;

        // Trả về Partial View chứa kết quả
        return PartialView("_SuggestionResultPartial", gameSuggestions); 
    }
    
    [Route("/{slug}")]
    public IActionResult Detail(string slug)
    {
        var game = _gameService.GetBySlug(slug);
        if (game == null)
        {
            return NotFound();
        }
        
        ViewBag.Screenshots = _gameService.GetScreenshotUrls(game.Id);
        return View("Detail", game); // trỏ tới Views/Game/Detail.cshtml
    }

    [HttpGet]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame()
    {
        // Lay danh sach the loai de hien thi o dropdown
        var categories = mDbContext.Category.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        // ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Categories = categories;
        return View(new GameCategory());
    }

    [HttpPost]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame(GameCategory gameCat, List<int> CategoryIds)
    {
        // Lay danh sach the loai de hien thi o dropdown vi ViewBag khong luu giu duoc giua cac lan post
        var categories = mDbContext.Category.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        ViewBag.Categories = categories;

        if (!ModelState.IsValid)
        {
            ModelState.Values.ToList().ForEach(v =>
            {
                v.Errors.ToList().ForEach(e => Console.WriteLine(e.ErrorMessage));
            });

            return View(gameCat);
        }

        DbUtils.InsertModel(gameCat.Game);

        foreach (var catId in CategoryIds)
        {
            var gc = new GameCategory
            {
                GameId = gameCat.Game.Id,
                CategoryId = catId
            };
            DbUtils.InsertModel(gc);
        }

        return View(new GameCategory());
    }

    [HttpGet]
    [Route("/Admin/EditGame/{id}")]
    public IActionResult EditGame(int id)
    {
        //var gameCat = mDbContext.GameCategory.FirstOrDefault(gc => gc.GameId == id);
        var gameCat = mDbContext.GameCategory
            .Include(g => g.Game)
            .Include(c => c.Category)
            .FirstOrDefault(gc => gc.GameId == id);

        var selectedCats = mDbContext.GameCategory
            .Where(gc => gc.GameId == id)
            .Select(gc => gc.CategoryId)
            .ToList();

        ViewBag.SelectedCategories = selectedCats;

        var categories = mDbContext.Category.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();
        Console.WriteLine("game cate: " + gameCat);
        ViewBag.Categories = categories;
        // ViewBag.Categories = new SelectList(categories, "Id", "Name");

        return View(gameCat);
    }

    [HttpPost]
    [Route("/Admin/EditGame/{id}")]
    public IActionResult EditGame(int id, GameCategory gameCat, List<int> CategoryIds)
    {
        if (!ModelState.IsValid)
        {
            return View(gameCat);
        }

        var game = mDbContext.GameCategory
            .Include(g => g.Game)
            .FirstOrDefault(gc => gc.Game.Id == id);

        game.Game.Title = gameCat.Game.Title;
        game.Game.Slug = gameCat.Game.Slug;
        game.Game.TitleImageUrl = gameCat.Game.TitleImageUrl;
        game.Game.ShortDescription = gameCat.Game.ShortDescription;
        game.Game.DetailedDescription = gameCat.Game.DetailedDescription;
        game.Game.BgImageUrl = gameCat.Game.BgImageUrl;
        game.Game.bPublic = gameCat.Game.bPublic;
        game.Game.TrailerUrl = gameCat.Game.TrailerUrl;
        game.Game.ItchioUrl = gameCat.Game.ItchioUrl;
        game.Game.EpicUrl = gameCat.Game.EpicUrl;
        game.Game.SteamUrl = gameCat.Game.SteamUrl;
        game.Game.FabUrl = gameCat.Game.FabUrl;
        game.Game.Price = gameCat.Game.Price;
        game.Game.DiscountPercent = gameCat.Game.DiscountPercent;
        game.Game.ReleaseDate = gameCat.Game.ReleaseDate;

        mDbContext.SaveChanges();

        // game.CategoryId = gameCat.CategoryId;

        //Xoa het the loai cua game
        var oldCat = mDbContext.GameCategory.Where(gc => gc.GameId == id).ToList();
        mDbContext.GameCategory.RemoveRange(oldCat);

        foreach (var item in CategoryIds)
        {
            mDbContext.GameCategory.Add(new GameCategory
            {
                GameId = id,
                CategoryId = item
            });
        }

        mDbContext.SaveChanges();

        return RedirectToAction("Admin", "Home");
    }

    [HttpGet]
    [Route("/Admin/DeleteGame/{id}")]
    public IActionResult DeleteGame(int id)
    {
        var model = mDbContext.Game.Find(id);
        return View(model);
    }

    [HttpPost]
    [Route("/Admin/DeleteGame/{id}")]
    public IActionResult DeleteGame(int id, Game game)
    {
        game = mDbContext.Game.Find(id);
        mDbContext.Game.Remove(game);
        mDbContext.SaveChanges();
        return RedirectToAction("Admin", "Home");
    }

    [HttpGet]
    [Route("/Admin/AddCategory")]
    public IActionResult AddCategory()
    {
        return View();
    }

    [HttpPost]
    [Route("/Admin/AddCategory")]
    public IActionResult AddCategory(Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        DbUtils.InsertModel(category);
        return View();
    }
    
    
}