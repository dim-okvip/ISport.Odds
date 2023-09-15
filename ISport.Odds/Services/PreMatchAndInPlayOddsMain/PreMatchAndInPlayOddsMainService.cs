namespace ISport.Odds.Services
{
    public class PreMatchAndInPlayOddsMainService : IPreMatchAndInPlayOddsMainService
    {
        private IPreMatchAndInPlayOddsMainRepository _repository;

        public PreMatchAndInPlayOddsMainService(IPreMatchAndInPlayOddsMainRepository preMatchAndInPlayOddsMainRepository)
        {
            _repository = preMatchAndInPlayOddsMainRepository;
        }

        public async Task<IEnumerable<PreMatchAndInPlayOddsMain>> GetAllAsync()
           => await _repository.GetAllAsync();

        public async Task<PreMatchAndInPlayOddsMain> GetByIdAsync(string id)
            => await _repository.GetByIdAsync(id);

        public async Task<PreMatchAndInPlayOddsMain> GetByMatchIdAsync(Source source, string id, string matchId, string companyId)
        {
            PreMatchAndInPlayOddsMain odds = new();
            switch (source)
            {
                case Source.MongoDB:
                    odds = await _repository.GetByIdAsync(id);
                    break;
                case Source.InMemory:
                    odds = InMemory.PreMatchAndInPlayOddsMain;
                    break;
                default:
                    odds = await _repository.GetByIdAsync(id);
                    break;
            }

            if (odds is not null && odds.Data is not null)
            {
                odds.Data.Handicap = Filter(odds.Data.Handicap, matchId, companyId);
                odds.Data.EuropeOdds = Filter(odds.Data.EuropeOdds, matchId, companyId);
                odds.Data.OverUnder = Filter(odds.Data.OverUnder, matchId, companyId);
                odds.Data.HandicapHalf = Filter(odds.Data.HandicapHalf, matchId, companyId);
                odds.Data.OverUnderHalf = Filter(odds.Data.OverUnderHalf, matchId, companyId);
            }
            return odds;
        }

        private List<string> Filter(List<string> list, string matchId, string? companyId)
        {
            List<string> result = list;
            if (!String.IsNullOrEmpty(matchId))
                result = result.Where(item => item.Split(",")[0] == matchId).ToList();

            if (!String.IsNullOrEmpty(companyId))
                result = result.Where(item => item.Split(",")[1] == companyId).ToList();
            return result;
        }

        public async Task<PreMatchAndInPlayOddsMain> InsertAsync(PreMatchAndInPlayOddsMain obj)
           => await _repository.InsertAsync(obj);

        public async Task<PreMatchAndInPlayOddsMain> UpdateAsync(string id, PreMatchAndInPlayOddsMain obj)
            => await _repository.UpdateAsync(id, obj);

        public async Task<bool> RemoveAsync(string id)
         => await _repository.RemoveAsync(id);
    }
}
