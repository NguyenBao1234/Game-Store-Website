using POWStudio.Models;
using POWStudio.Models.Enum;

namespace POWStudio.Services;

public interface IGameService
{
    Game? GetBySlug(string slug);
    public IQueryable<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false);
    IQueryable<Game> GetAll();
    IQueryable<Category> GetAllCategories();
    IQueryable<Game> GetGamesBySortOption(IQueryable<Game> inGamesQuery, GameSortOption sortOption = GameSortOption.All, bool isAscending = true, int inLimitAmount = 20, bool inGetAll = false);
    IQueryable<Game> GetGamesByCategory(IQueryable<Game> inGameQuery, List<int> inCategoryIds);
    IQueryable <Game> GetGamesByPriceRange(IQueryable<Game> inGamesQuery, decimal? min, decimal? max);
    List<string> GetScreenshotUrls(int inGameId);
    public IQueryable<Rate> GetRates(int inGameId);
}