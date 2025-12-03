using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POWStudio.Models;
using POWStudio.Services;

namespace POWStudio.Controllers;

public class PaymentController : Controller
{
    private readonly IGameService _gameService;
    private readonly GameStoreDbContext mDbContext;
    private readonly UserManager<ApplicationUser> mUserManager;

    public PaymentController(UserManager<ApplicationUser> userManager, IGameService gameService,
        GameStoreDbContext mDbContextContext)
    {
        mUserManager = userManager;
        _gameService = gameService;
        mDbContext = mDbContextContext;
    }

    public IActionResult Cart()
    {
        var userId = mUserManager.GetUserId(User);
        var cartItems = _gameService.GetCartItems(userId).ToList();

        decimal totalDiscount = 0;
        decimal totalPrice = 0;
        decimal subTotal = 0;
        foreach (var cartItem in cartItems)
        {
            var game = cartItem.Game;
            var discountPercent = game.DiscountPercent ?? 0;
            var price = game.Price ?? 0;
            totalDiscount += price * (decimal)discountPercent / 100;
            totalPrice += price;
        }

        subTotal = totalPrice - totalDiscount;
        ViewBag.SubTotal = subTotal;
        ViewBag.TotalPrice = totalPrice;
        ViewBag.TotalDiscount = totalDiscount;
        Console.WriteLine($"Price : {totalPrice}, Discount : {totalDiscount},  Sub total : {subTotal}");
        return View(cartItems);
    }

    public IActionResult AddToCart(int gameId)
    {
        var gameSlug = _gameService.GetGameById(gameId).Slug;
        var userId = mUserManager.GetUserId(User);
        var cartId = _gameService.GetCartId(userId);
        _gameService.AddToCart(gameId, cartId);

        return RedirectToAction("Detail", "Game", new { slug = gameSlug });
    }

    public IActionResult Remove(int cartItemId)
    {
        var cartItem = mDbContext.CartItem.FirstOrDefault(c => c.Id == cartItemId);

        if (cartItem != null)
        {
            mDbContext.CartItem.Remove(cartItem);
            mDbContext.SaveChanges();
        }

        return RedirectToAction("Cart");
    }

    public IActionResult AddToCartFromWishlist(int gameId)
    {
        var gameSlug = _gameService.GetGameById(gameId).Slug;
        var userId = mUserManager.GetUserId(User);
        var cartId = _gameService.GetCartId(userId);
        _gameService.AddToCart(gameId, cartId);

        return RedirectToAction("Wishlist", "Game");
    }
    
}
    