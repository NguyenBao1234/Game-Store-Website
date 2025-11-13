using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Services;
using POWStudio.Utils;

namespace POWStudio.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;
    private readonly GameStoreDbContext db;

    public GameController(IGameService gameService, GameStoreDbContext dbContext)
    {
        _gameService = gameService;
        db = dbContext;
    }

    [Route("/{slug}")]
    public IActionResult Detail(string slug)
    {
        var game = _gameService.GetBySlug(slug);
        if (game == null)
        {
            return NotFound();
        }

        return View("Detail", game); // trỏ tới Views/Game/Detail.cshtml
    }

    [HttpGet]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame()
    {
        var categories = db.Category.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame(GameCategory gameCat)
    {
        if (!ModelState.IsValid)
        {
            ModelState.Values.ToList().ForEach(v =>
            {
                v.Errors.ToList().ForEach(e => Console.WriteLine(e.ErrorMessage));
            });
            return View(gameCat);
        }

        DbUtils.InsertModel(gameCat);
        return View();
    }

    [HttpGet]
    [Route("/Admin/EditGame/{id}")]
    public IActionResult EditGame(int id)
    {
        var gameCat = db.GameCategory
            .Include(g => g.Game)
            .Include(c => c.Category)
            .FirstOrDefault(gc => gc.GameId == id);
        var categories = db.Category.ToList();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(gameCat);
    }

    [HttpPost]
    [Route("/Admin/EditGame/{id}")]
    public IActionResult EditGame(int id, GameCategory gameCat)
    {
        if (!ModelState.IsValid)
        {
            return View(gameCat);
        }

        var game = db.GameCategory
            .Include(g => g.Game)
            .FirstOrDefault(gc => gc.GameId == id);

        game.Game.Title = gameCat.Game.Title;
        game.Game.Slug = gameCat.Game.Slug;
        game.Game.TittleImageUrl = gameCat.Game.TittleImageUrl;
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
        
        game.CategoryId = gameCat.CategoryId;
        
        db.SaveChanges();

        return RedirectToAction("Admin", "Home");
    }

    [HttpGet]
    [Route("/Admin/DeleteGame/{id}")]
    public IActionResult DeleteGame(int id)
    {
        var model = db.Game.Find(id);
        return View(model);
    }

    [HttpPost]
    [Route("/Admin/DeleteGame/{id}")]
    public IActionResult DeleteGame(int id, Game game)
    {
        game = db.Game.Find(id);
        db.Game.Remove(game);
        db.SaveChanges();
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