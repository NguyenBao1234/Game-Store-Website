using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Models.Enum;

namespace POWStudio.Services;
using System.Collections.Generic;
using System.Linq;


public class GameService : IGameService
{
    private readonly GameStoreDbContext mDBContext;

    //inject
    public GameService(GameStoreDbContext mDbContext)
    {
        mDBContext = mDbContext;
    }
    

    public Game? GetBySlug(string slug)
    {
        return mDBContext.Game.FirstOrDefault(g => g.Slug == slug);
    }

    public Game? GetGameById(int gameId)
    {
        return mDBContext.Game.FirstOrDefault(g => g.Id == gameId);
    }

    public IQueryable<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false)
    {
        if (string.IsNullOrEmpty(term))
        {
            return mDBContext.Game.Take(inLimitAmount);
        }
        IQueryable<Game> query = mDBContext.Game.Where(g => g.Title.Contains(term));
        
        if (!inGetAll && inLimitAmount > 0) query = query.Take(inLimitAmount);
        return query;
    }

    public IQueryable<Game> GetGamesBySortOption(IQueryable<Game> inGamesQuery, GameSortOption sortOption = GameSortOption.All, bool isAscending = true,
        int inLimitAmount = 20, bool inGetAll = false)
    {
        IQueryable<Game> sortGameQuery = inGamesQuery;

        switch (sortOption)
        {
            case GameSortOption.Price:
                sortGameQuery = isAscending ? sortGameQuery.OrderBy(g => g.Price) : sortGameQuery.OrderByDescending(g => g.Price);
                break;

            case GameSortOption.NewRelease:
                sortGameQuery = isAscending ? sortGameQuery.OrderBy(g => g.ReleaseDate) : sortGameQuery.OrderByDescending(g => g.ReleaseDate);
                break;

            case GameSortOption.Alphabetical:
                sortGameQuery = isAscending ? sortGameQuery.OrderBy(g => g.Title) : sortGameQuery.OrderByDescending(g => g.Title);
                break;
            
            case GameSortOption.ComingSoon:
                sortGameQuery = sortGameQuery.Where(g => g.ReleaseDate == null);
                sortGameQuery = isAscending ? sortGameQuery.OrderBy(g => g.Title) : sortGameQuery.OrderByDescending(g => g.Title);
                break;

            case GameSortOption.All:
            default:
                sortGameQuery = isAscending ? sortGameQuery.OrderBy(g => g.Id) : sortGameQuery.OrderByDescending(g => g.Id);
                break;
        }
        if (inGetAll) sortGameQuery = sortGameQuery.Take(inLimitAmount);
        return sortGameQuery;
    }

    public IQueryable<Game> GetGamesByCategory(IQueryable<Game> inGameQuery, List<int> inCategoryIds)
    {
        if (inCategoryIds == null || inCategoryIds.Count == 0) return inGameQuery;
        
        var selectedCategoriesQuery = mDBContext.GameCategory.Where(gc => inCategoryIds.Contains(gc.CategoryId));
        var result = from g in inGameQuery join gc in selectedCategoriesQuery on g.Id equals gc.GameId
            select g;

        return result.Distinct();
    }

    public IQueryable<Game> GetGamesByPriceRange(IQueryable<Game> inGamesQuery, decimal? min, decimal? max)
    {
        if (min == 0) inGamesQuery = inGamesQuery.Where(g => (g.Price == null) || (g.Price >= min && g.Price <= max));
        else inGamesQuery = inGamesQuery.Where(g => g.Price != null && g.Price >= min && g.Price <= max);

        return inGamesQuery;
    }
    public IQueryable<Game> GetAll()
    {
        return mDBContext.Game;
    }

    public IQueryable<Category> GetAllCategories()
    {
        return mDBContext.Category;
    }

    public List<string> GetScreenshotUrls(int inGameId)
    {
        return mDBContext.GameScreenshot.Where(g => g.GameId == inGameId).Select(gs=>gs.ScreenshotUrl).ToList();
    }

    public IQueryable<Rate> GetRates(int inGameId)
    {
        return mDBContext.Rate.Where(r => r.GameId == inGameId).Include(r=>r.User);
    }

    public int GetCartId(string inUserId)
    {
        var cart = mDBContext.Cart.FirstOrDefault(c => c.UserId == inUserId);
        if (cart != null) return cart.Id;
        cart = new Cart
        {
            UserId = inUserId
        };
        
        mDBContext.Cart.Add(cart);
        mDBContext.SaveChanges();

        return cart.Id;
    }
    public bool IsGameInCart(int inGameId, string inUserId)
    {
        var cartId = GetCartId(inUserId);
        return mDBContext.CartItem.Any(ci=>ci.GameId == inGameId && ci.CartId == cartId);
    }

    public IQueryable<CartItem> GetCartItems(string inUserId)
    {
        var cartId = GetCartId(inUserId);
        return mDBContext.CartItem.Where(ci=> ci.CartId == cartId).Include(ci=>ci.Game);
    }

    public void AddToCart(int gameId, int inCartId)
    {
        var bGameExist = mDBContext.CartItem.Any(ci => ci.GameId == gameId && ci.CartId == inCartId);
        if (bGameExist) return;
        mDBContext.CartItem.Add(new CartItem {GameId = gameId, CartId = inCartId});
        mDBContext.SaveChanges();
    }

    public void RemoveCartItem(int inGameId, int inCartId)
    {
        var cartItem = mDBContext.CartItem.FirstOrDefault(c => c.CartId ==  inCartId && c.GameId == inGameId);

        if (cartItem != null)
        {
            mDBContext.CartItem.Remove(cartItem);
            mDBContext.SaveChanges();
        }
    }

    public int GetWishlistId(string inUserId)
    {
        var wishlist = mDBContext.Wishlist.FirstOrDefault(w => w.UserId == inUserId);
        if (wishlist != null) return wishlist.Id;
        wishlist = new Wishlist
        {
            UserId = inUserId
        };
        
        mDBContext.Wishlist.Add(wishlist);
        mDBContext.SaveChanges();

        return wishlist.Id;
    }

    public bool IsGameInWishlist(int inGameId, string inUserId)
    {
        var wishlistId = GetWishlistId(inUserId);
        return mDBContext.WishlistItem.Any(wi=>wi.GameId == inGameId && wi.WishlistId == wishlistId);
    }

    public IQueryable<WishlistItem> GetWishlistItems(string inUserId)
    {
        var wishlistId = GetWishlistId(inUserId);
        return mDBContext.WishlistItem.Where(wi => wi.WishlistId == wishlistId).Include(wi=>wi.Game);
    }

    public void AddToWishlist(int gameId, int inWishlistId)
    {
        var bGameInWishlist = mDBContext.WishlistItem.Any(wi => wi.GameId == gameId && wi.WishlistId == inWishlistId);
        if (bGameInWishlist) return;
        mDBContext.WishlistItem.Add(new WishlistItem{GameId = gameId, WishlistId = inWishlistId});
        mDBContext.SaveChanges();
    }

    public int GetLibraryId(string inUserId)
    {
        var uLibrary = mDBContext.UserLibrary.FirstOrDefault(l => l.UserId == inUserId);
        if (uLibrary != null) return uLibrary.Id;
        uLibrary = new UserLibrary
        {
            UserId = inUserId
        };
        mDBContext.UserLibrary.Add(uLibrary);
        mDBContext.SaveChanges();
        
        return uLibrary.Id;
    }

    public bool IsGameInLibrary(int inGameId, string inUserId)
    {
        var LibID = GetLibraryId(inUserId);
        return mDBContext.LibraryItem.Any(li => li.GameId == inGameId && li.LibraryId == LibID);
    }

    public void AddLibraryItem(int gameId, int inLibraryId)
    {
        var bExist = mDBContext.LibraryItem.Any(li => li.GameId == gameId && li.LibraryId == inLibraryId);
        if (bExist) return;
        
        var LibItem = new LibraryItem
        {
            GameId = gameId,
            LibraryId = inLibraryId
        };
        mDBContext.LibraryItem.Add(LibItem);
        mDBContext.SaveChanges();
    }

    public int AddNewOrder(string inUserId, decimal price, decimal? discountAmount)
    {
        var order = new Order
        {
            OrderDate = DateTime.Now,
            Price = price,
            DiscountAmount = discountAmount
        };
        mDBContext.Order.Add(order);
        mDBContext.SaveChanges();
        var userOrder = new UserOrder
        {
            OrderId = order.Id,
            UserId = inUserId
        };

        mDBContext.UserOrder.Add(userOrder);
        mDBContext.SaveChanges();
        return order.Id;
    }

    public void AddOrderItem(int gameId, int inOrderId)
    {
        var bExist = mDBContext.OrderItem.Any(li => li.GameId == gameId && li.OrderId == inOrderId);
        if (bExist) return;
        mDBContext.OrderItem.Add(new OrderItem {GameId = gameId, OrderId = inOrderId});
        mDBContext.SaveChanges();
    }
}