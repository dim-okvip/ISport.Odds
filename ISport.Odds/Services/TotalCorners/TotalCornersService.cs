namespace ISport.Odds.Services
{
    public class TotalCornersService : ITotalCornersService
    {
        private readonly ITotalCornersRepository _repository;

        public TotalCornersService(ITotalCornersRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TotalCorners>> GetAllAsync()
           => await _repository.GetAllAsync();

        public async Task<TotalCorners> GetByIdAsync(string id)
            => await _repository.GetByIdAsync(id);

        public async Task<TotalCorners> GetByMatchIdAsync(Source source, string id, string? matchId, string? companyId)
        {
            TotalCorners odds = new();
            switch (source)
            {
                case Source.MongoDB:
                    odds = await _repository.GetByIdAsync(id);
                    break;
                case Source.InMemory:
                    if (id == Utils.TotalCornersPreMatchId) odds = InMemory.TotalCornersPreMatch.Clone();
                    if (id == Utils.TotalCornersInPlayId) odds = InMemory.TotalCornersInPlay.Clone();
                    break;
                default:
                    odds = await _repository.GetByIdAsync(id);
                    break;
            }

            if (odds is not null && odds.Data is not null)
                odds.Data = Filter(odds.Data, matchId, companyId);

            return odds;
        }

        private List<TotalCornersDetail> Filter(List<TotalCornersDetail> list, string? matchId, string? companyId)
        {
            List<TotalCornersDetail> result = list;
            if (!String.IsNullOrEmpty(matchId))
                result = result.Where(item => item.MatchId == matchId).ToList();

            if (!String.IsNullOrEmpty(companyId))
                result = result.Where(item => item.CompanyId == companyId).ToList();
            return result;
        }

        public async Task<TotalCorners> InsertAsync(TotalCorners obj)
            => await _repository.InsertAsync(obj);

        public async Task<TotalCorners> UpdateAsync(string id, TotalCorners obj)
            => await _repository.UpdateAsync(id, obj);

        public async Task<bool> RemoveAsync(string id)
           => await _repository.RemoveAsync(id);
    }
}
