using POWStudio.Models;

namespace POWStudio.Services;

public interface IGameService
{
    Game? GetBySlug(string slug);
    public List<Game> GetGamesByTerm(string term, int inLimitAmount = 4, bool inGetAll = false);
    List<Game> GetAll();
}