namespace ISport.Odds.Services
{
    public interface IPreMatchAndInPlayOddsMainService
    {
        Task<IEnumerable<PreMatchAndInPlayOddsMain>> GetAllAsync();
        Task<PreMatchAndInPlayOddsMain> GetByIdAsync(string id);
        Task<PreMatchAndInPlayOddsMain> GetByMatchIdAsync(Source source, string id, string matchId, string companyId);
        Task<PreMatchAndInPlayOddsMain> InsertAsync(PreMatchAndInPlayOddsMain obj);
        Task<PreMatchAndInPlayOddsMain> UpdateAsync(string id, PreMatchAndInPlayOddsMain obj);
        Task<bool> RemoveAsync(string id);
    }
}
