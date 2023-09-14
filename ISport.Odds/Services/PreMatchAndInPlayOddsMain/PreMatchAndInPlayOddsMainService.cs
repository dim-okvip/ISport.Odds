namespace ISport.Odds.Services
{
    public class PreMatchAndInPlayOddsMainService : IPreMatchAndInPlayOddsMainService
    {
        private IPreMatchAndInPlayOddsMainRepository _repository;

        public PreMatchAndInPlayOddsMainService(IPreMatchAndInPlayOddsMainRepository preMatchAndInPlayOddsMainRepository)
        {
            _repository = preMatchAndInPlayOddsMainRepository;
        }

        public async Task<IEnumerable<PreMatchAndInPlayOddsMain>> GetAll()
           => await _repository.GetAll();

        public async Task<PreMatchAndInPlayOddsMain> GetByIdAsync(string id)
            => await _repository.GetById(id);

        public async Task<PreMatchAndInPlayOddsMain> GetByMatchIdAsync(string id, string matchId, string? companyId)
        {
            PreMatchAndInPlayOddsMain odds = await _repository.GetById(id);
            if (odds is not null)
            {
                odds.Data.Handicap = FilterList(odds.Data.Handicap, matchId, companyId);
                odds.Data.EuropeOdds = FilterList(odds.Data.EuropeOdds, matchId, companyId);
                odds.Data.OverUnder = FilterList(odds.Data.OverUnder, matchId, companyId);
                odds.Data.HandicapHalf = FilterList(odds.Data.HandicapHalf, matchId, companyId);
                odds.Data.OverUnderHalf = FilterList(odds.Data.OverUnderHalf, matchId, companyId);
                return odds;
            }
            return new PreMatchAndInPlayOddsMain();
        }

        private List<string> FilterList(List<string> list, string matchId, string companyId)
        {
            List<string> result = list;
            if (!String.IsNullOrEmpty(matchId))
                result = result.Where(item => item.Split(",")[0] == matchId).ToList();

            if (!String.IsNullOrEmpty(companyId))
                result = result.Where(item => item.Split(",")[1] == companyId).ToList();
            return result;
        }

        public async Task<PreMatchAndInPlayOddsMain> InsertAsync(PreMatchAndInPlayOddsMain obj)
           => await _repository.Insert(obj);

        public async Task<PreMatchAndInPlayOddsMain> UpdateAsync(string id, PreMatchAndInPlayOddsMain obj)
            => await _repository.Update(id, obj);

        public async Task<bool> RemoveAsync(string id)
         => await _repository.Remove(id);
    }
}
