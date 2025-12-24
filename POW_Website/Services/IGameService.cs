using POWStudio.Models;
using POWStudio.Models.Enum;

namespace POWStudio.Services;

public interface IGameService
{
    Game? GetBySlug(string slug);
    Game? GetGameById(int gameId);
    public IQueryable<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false);
    public IQueryable<Game> GetLibGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false, string inUserId = null);
    IQueryable<Game> GetAll();
    IQueryable<Game?> GetSpotlightGames();
    IQueryable<Category> GetAllCategories();
    IQueryable<Game> GetGamesBySortOption(IQueryable<Game> inGamesQuery, GameSortOption sortOption = GameSortOption.All, bool isAscending = true, int inLimitAmount = 20, bool inGetAll = false);
    IQueryable<Game> GetGamesByCategory(IQueryable<Game> inGameQuery, List<int> inCategoryIds);
    IQueryable <Game> GetGamesByPriceRange(IQueryable<Game> inGamesQuery, decimal? min, decimal? max);
    List<string> GetScreenshotUrls(int inGameId);
    public IQueryable<Rate> GetRates(int inGameId);
    public int GetCartId(string inUserId);
    public bool IsGameInCart(int inGameId, string inUserId);
    public IQueryable<CartItem> GetCartItems(string inUserId);

    void AddToCart(int gameId, int inCartId);
    void RemoveCartItem(int inGameId, int inCartId);
    
    public int GetWishlistId(string inUserId);
    public bool IsGameInWishlist(int inGameId, string inUserId);
    public IQueryable<WishlistItem> GetWishlistItems(string inUserId);
    void AddToWishlist(int gameId, int inWishlistId);
    
    public int GetLibraryId(string inUserId);
    public bool IsGameInLibrary(int inGameId, string inUserId);
    void AddLibraryItem(int gameId, int inLibraryId);
    public int AddNewOrder(string inUserId, decimal price, decimal? discountAmount);
    void AddOrderItem(int gameId, int inOrderId);
    
    IQueryable<Game> GetLibraryGames(string inUserId);
}