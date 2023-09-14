namespace ISport.Odds.Services
{
    public interface ITotalCornersService
    {
        Task<IEnumerable<TotalCorners>> GetAll();
        Task<TotalCorners> GetByIdAsync(string id);
        Task<TotalCorners> GetByMatchIdAsync(string id, string matchId, string? companyId);
        Task<TotalCorners> InsertAsync(TotalCorners obj);
        Task<TotalCorners> UpdateAsync(string id, TotalCorners obj);
        Task<bool> RemoveAsync(string id);
    }
}
