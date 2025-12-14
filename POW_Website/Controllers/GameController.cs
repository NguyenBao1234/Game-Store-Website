using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Models.Enum;
using POWStudio.Models.ViewModels;
using POWStudio.Services;
using POWStudio.Utils;

namespace POWStudio.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;
    private readonly GameStoreDbContext mDbContext;
    private readonly UserManager<ApplicationUser> mUserManager;
    public GameController(UserManager<ApplicationUser> userManager, IGameService gameService, GameStoreDbContext mDbContextContext)
    {
        mUserManager = userManager;
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
        ViewBag.UserId =  mUserManager.GetUserId(User);
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

        var rates = _gameService.GetRates(game.Id).ToList();
        int PositiveAmout = 0;
        int  NegativeAmout = 0;
        foreach (var rate in rates)
        {
            if (rate.bRecomended) PositiveAmout++;
            else NegativeAmout++;
        }
        var sumRate = PositiveAmout + NegativeAmout; 
        double score = sumRate == 0 ? 0 : (double)PositiveAmout / sumRate;
        JudgedRating judgedRating;
        if (sumRate >= 500)
        {
            if (score >= 0.95) judgedRating = JudgedRating.OverwhelminglyPositive;
            else if (score >= 0.80) judgedRating = JudgedRating.VeryPositive;
            else if (score >= 0.70) judgedRating = JudgedRating.MostlyPositive;
            else if (score >= 0.40) judgedRating = JudgedRating.Mixed;
            else if (score >= 0.20) judgedRating = JudgedRating.MostlyNegative;
            else if (score >= 0.05) judgedRating = JudgedRating.VeryNegative;
            else judgedRating = JudgedRating.OverwhelminglyNegative;
        }
        else if (sumRate >= 50)
        {
            if (score >= 0.95) judgedRating = JudgedRating.VeryPositive;
            else if (score >= 0.80) judgedRating = JudgedRating.VeryPositive;
            else if (score >= 0.70) judgedRating = JudgedRating.MostlyPositive;
            else if (score >= 0.40) judgedRating = JudgedRating.Mixed;
            else if (score >= 0.20) judgedRating = JudgedRating.MostlyNegative;
            else judgedRating = JudgedRating.VeryNegative;
        }
        else if (sumRate == 0) judgedRating = JudgedRating.None;
        else 
        {
            if (score >= 0.80) judgedRating = JudgedRating.Positive;
            else if (score >= 0.40) judgedRating = JudgedRating.Mixed;
            else judgedRating = JudgedRating.Negative;
        }

        var judgedRatingString = judgedRating.ToString();
        switch (judgedRating)
        {
            case JudgedRating.OverwhelminglyNegative:
                judgedRatingString = "Overwhelmingly Negative";
                break;
            case JudgedRating.VeryNegative:
                judgedRatingString = "Very Negative";
                break;
            case JudgedRating.MostlyNegative:
                judgedRatingString = "Mostly Negative";
                break;
            case JudgedRating.MostlyPositive:
                judgedRatingString = "Mostly Positive";
                break;
            case JudgedRating.VeryPositive:
                judgedRatingString = "Very Positive";
                break;
            case JudgedRating.OverwhelminglyPositive:
                judgedRatingString = "Overwhelmingly Positive";
                break;
            default:
                break;
        }
        var userId = mUserManager.GetUserId(User);
        ViewBag.JudgedRating = (int)judgedRating;
        ViewBag.JudgedRatingString = judgedRatingString;
        ViewBag.Screenshots = _gameService.GetScreenshotUrls(game.Id);
        ViewBag.UserId = userId;
        ViewBag.bGameInCart = userId != null && _gameService.IsGameInCart(game.Id,userId);
        ViewBag.bGameInWishlist = userId != null && _gameService.IsGameInWishlist(game.Id,userId);
        ViewBag.bOwned = _gameService.IsGameInLibrary(game.Id, userId);
        return View("Detail", new GameDetailModel{Game = game, Rates = rates }); // trỏ tới Views/Game/Detail.cshtml
    }

    public IActionResult Wishlist()
    {
        var userId = mUserManager.GetUserId(User);
        var wishlist = _gameService.GetWishlistItems(userId).ToList();
        ViewBag.UserId = userId;
        return View(wishlist);
    }

    public IActionResult RemoveWishlist(int wishItemId)
    {
        var wishlistItem = mDbContext.WishlistItem.FirstOrDefault(wi=> wi.Id== wishItemId);
        if (wishlistItem != null)
        {
            mDbContext.WishlistItem.Remove(wishlistItem);
            var sum = mDbContext.SaveChanges();
        }
        return RedirectToAction("Wishlist");
    }

    public IActionResult AddToWishlistFromDetail(int gameId)
    {
        
        var gameSlug = _gameService.GetGameById(gameId).Slug;
        var userId = mUserManager.GetUserId(User);
        var cartId = _gameService.GetCartId(userId);
        _gameService.AddToWishlist(gameId, cartId);

        return RedirectToAction("Detail", "Game", new { slug = gameSlug });
    }    
    public IActionResult AddToWishlistFromSearch(int gameId)
    {
        var gameSlug = _gameService.GetGameById(gameId).Slug;
        var userId = mUserManager.GetUserId(User);
        var cartId = _gameService.GetCartId(userId);
        _gameService.AddToWishlist(gameId, cartId);

        return RedirectToAction("Index");
    }
    public IActionResult RemoveWishlistFromSearch(int wishItemId)
    {
        var wishlistItem = mDbContext.WishlistItem.FirstOrDefault(wi=> wi.Id== wishItemId);
        if (wishlistItem != null)
        {
            mDbContext.WishlistItem.Remove(wishlistItem);
            var sum = mDbContext.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Order(int page = 1, int pageSize = 10)
    {
        var orders = mDbContext.Order.OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var total = mDbContext.Order.Count();

        var orderList = new List<OrderUListItemVM>();
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

            var mainName = itemsInOrder[0].Name;
            var subName = itemsInOrder.Count == 1 ? "" : $"and {itemsInOrder.Count - 1} more";

            orderList.Add(new OrderUListItemVM
            {
                OrderId = order.Id,
                OriginalPrice = order.Price,
                DiscountAmount = order.DiscountAmount,
                MainName = mainName,
                SubName = subName,
                OrderDate = order.OrderDate,
                Games = itemsInOrder,
            });
        }

        var orderVM = new OrderUVM
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            OrderListItemVms = orderList
        };

        return View(orderVM);
    }
}