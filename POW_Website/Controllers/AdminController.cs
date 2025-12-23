using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Models.ViewModels;
using POWStudio.Services;
using POWStudio.Utils;

namespace POWStudio.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IGameService _gameService;
    private readonly SignInManager<ApplicationUser> mSignInManager;
    private readonly GameStoreDbContext mDbContext;
    public AdminController(ILogger<HomeController> logger, IGameService gameService,
        SignInManager<ApplicationUser> inSignInManager, GameStoreDbContext mDbContextContext)
    {
        _gameService = gameService;
        mSignInManager = inSignInManager;
        _logger = logger;
        mDbContext = mDbContextContext;
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

    public IActionResult Order(int page = 1, int pageSize = 10)
    {
        var orders = mDbContext.Order.OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var total = mDbContext.Order.Count();

        var orderList = new List<OrderListItemVM>();
        foreach (var order in orders)
        {
            var itemsInOrder = mDbContext.OrderItem
                .Where(x => x.OrderId == order.Id)
                .Select(x => new GameInOrderVM
                {
                    Name = x.Game.Title,
                    Price = x.Game.Price??0,
                })
                .ToList();
            var userOrder = mDbContext.UserOrder.FirstOrDefault(uo=>uo.OrderId == order.Id);
            var user = mDbContext.Users.FirstOrDefault(u=>u.Id == userOrder.UserId);
            var userDisplayName = user.DisplayName;
            var userEmail = user.Email;
            var mainName = itemsInOrder[0].Name;
            var subName = itemsInOrder.Count == 1 ? "" : $"and {itemsInOrder.Count - 1} more";

            orderList.Add(new OrderListItemVM
            {
                OrderId = order.Id,
                OriginalPrice = order.Price,
                DiscountAmount = order.DiscountAmount,
                MainName = mainName,
                SubName = subName,
                OrderDate = order.OrderDate,
                Games = itemsInOrder,
                UserDisplayName = userDisplayName,
                UserEmail = userEmail,
            });
        }

        var orderVM = new OrderVM
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            OrderListItemVms = orderList
        };

        return View(orderVM);
    }

    public IActionResult SaleReport(ReportSaleVM filter, string ActionType)
    {
        var totalOrder = mDbContext.Order;
        filter.AllTimeRevenue = mDbContext.Order.Sum(o => o.Price - o.DiscountAmount ?? 0);
        var availableYears = totalOrder
            .Select(o => o.OrderDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();
        
        if (availableYears == null || !availableYears.Any())
        {
            availableYears = new List<int> { DateTime.Now.Year };
        }
        ViewBag.AvailableYears = availableYears;
        List<Order> orders =  new List<Order>();
        
        if (ActionType == "Daily")
        {
            var date = filter.Date;
            var start = date.ToDateTime(TimeOnly.MinValue); // 00:00:00
            var end   = date.ToDateTime(TimeOnly.MaxValue); // 23:59:59.9999999
            orders = mDbContext.Order.Where(o => o.OrderDate >= start && o.OrderDate <= end).ToList();
            filter.DateRevenue = orders.Sum(o => (o.Price - o.DiscountAmount??0));
        }
        else if (ActionType == "Monthly")
        {
            var month = filter.Month;
            var start = month.ToDateTime(TimeOnly.MinValue);
            var end   = start.AddMonths(1);
            orders = mDbContext.Order.Where(o => o.OrderDate >= start && o.OrderDate < end).ToList();
            filter.MonthRevenue = orders.Sum(o => (o.Price - o.DiscountAmount??0));
        }
        else if (ActionType == "Yearly")
        {
            var year = filter.Year;
            var start = new DateTime(year, 1, 1);
            var end   = start.AddYears(1);
            orders = mDbContext.Order.Where(o => o.OrderDate >= start && o.OrderDate < end).ToList();
            filter.YearRevenue = orders.Sum(o => (o.Price - o.DiscountAmount??0));
        }
        
        return View(filter);
    }

    public IActionResult Rating()
    {
        throw new NotImplementedException();
    }
}