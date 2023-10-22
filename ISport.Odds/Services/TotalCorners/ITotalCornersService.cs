namespace ISport.Odds.Services
{
    public interface ITotalCornersService
    {
        Task<List<TotalCorners>> GetAllAsync();
        Task<TotalCorners> GetByIdAsync(string id);
        Task<TotalCorners> GetByMatchIdAsync(Source source, string id, string? matchId, string? companyId);
        Task<TotalCorners> InsertAsync(TotalCorners obj);
        Task<TotalCorners> UpdateAsync(string id, TotalCorners obj);
        Task<bool> RemoveAsync(string id);
    }
}
