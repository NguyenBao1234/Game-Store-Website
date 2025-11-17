using POWStudio.Models;
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

    private object _gameRepository;

    public Game? GetBySlug(string slug)
    {
        return mDBContext.Game.FirstOrDefault(g => g.Slug == slug);
    }
    
    public List<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false)
    {
        if (string.IsNullOrEmpty(term))
        {
            return new List<Game>();
        }
        IQueryable<Game> query = mDBContext.Game.Where(g => g.Title.Contains(term));
        
        if (!inGetAll && inLimitAmount > 0) query = query.Take(inLimitAmount);
        return query.ToList();
    }

    public List<Game> GetAll()
    {
        return mDBContext.Game.ToList();
    }
}