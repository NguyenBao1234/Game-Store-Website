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
        // TẠM THỜI: không gọi DB nữa
        // var games = _gameService.GetAll();

        // Trả về danh sách game rỗng (đúng kiểu model mà View đang cần)
        var games = new List<Game>();

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

    [Route("/Admin")]
    public IActionResult Admin()
    {
        try
        {
            //var models = mDbContext.GameCategory.Include(g => g.Game).Include(g => g.Category).ToList();
            var games = _gameService.GetAll().ToList();
            if (games.Count == 0) ViewBag.Message = "No data in GameCategory.";
            return View(games);
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Cannot connect to SQL Server in Admin()");
            return RedirectToAction("Error", new { message = "Cannot connect to database. Check connection string and SQL Server service." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Admin()");
            return RedirectToAction("Error", new { message = "Unhandled error." });
        }
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