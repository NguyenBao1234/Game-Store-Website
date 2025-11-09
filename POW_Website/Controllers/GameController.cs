using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POWStudio.Models;
using POWStudio.Services;
using POWStudio.Utils;

namespace POWStudio.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    [Route("/{slug}")]
    public IActionResult Detail(string slug)
    {
        var game = _gameService.GetBySlug(slug);
        if (game == null)
        {
            return NotFound();
        }

        return View("Detail",game); // trỏ tới Views/Game/Detail.cshtml
    }

    [HttpGet]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame()
    {
        return View();
    }

    [HttpPost]
    [Route("/Admin/AddGame")]
    public IActionResult AddGame(Game game)
    {
        if (!ModelState.IsValid)
        {
            Console.Write("Error");
            ModelState.Values.ToList().ForEach(v =>
            {
                v.Errors.ToList().ForEach(e => Console.WriteLine(e.ErrorMessage));
            });
            return View(game);
        }
        DbUtils.InsertModel(game);

        return View(new Game());
    }
}