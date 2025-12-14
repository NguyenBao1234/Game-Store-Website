using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Services;

namespace POWStudio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IGameService _gameService;
    private readonly SignInManager<ApplicationUser> mSignInManager;
    private readonly GameStoreDbContext mDbContext;

    public HomeController(ILogger<HomeController> logger, IGameService gameService,
        SignInManager<ApplicationUser> inSignInManager, GameStoreDbContext mDbContextContext)
    {
        _gameService = gameService;
        mSignInManager = inSignInManager;
        _logger = logger;
        mDbContext = mDbContextContext;
    }

    public IActionResult Index()
    {
        var games = _gameService.GetAll();
        return View(games);
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
    public IActionResult Error(string? message = null)
    {
        ViewBag.Message = message ?? "Unhandled Error.";
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Privacy()
    {
        return View();
    }
}