using POWStudio.Models;
using POWStudio.Models.Enum;

namespace POWStudio.Services;

public interface IGameService
{
    Game? GetBySlug(string slug);
    public IQueryable<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false);
    IQueryable<Game> GetAll();

    public IQueryable<Game> GetGamesBySortOption(IQueryable<Game> inGamesQuery, GameSortOption sortOption = GameSortOption.All, bool isAscending = true, int inLimitAmount = 20, bool inGetAll = false);
}