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
    public IQueryable<Game> GetAll()
    {
        return mDBContext.Game;
    }
    
}