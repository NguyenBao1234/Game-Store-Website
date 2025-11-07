using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POWStudio.Models;
using POWStudio.Services;

namespace POWStudio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IGameService _gameService;
    public HomeController(ILogger<HomeController> logger, IGameService gameService)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var games = _gameService.GetAll();
        return View(games);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }
    [Route("/About")]
    public IActionResult AboutPow()
    {
        return View();
    }
    [Route("/Contact")]
    public IActionResult Contact()
    {
        return View();
    }
    
    [Route("/WishList")]
    public IActionResult WishList()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}