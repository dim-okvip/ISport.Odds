namespace ISport.Odds.Services
{
    public class TotalCornersService : ITotalCornersService
    {
        private readonly ITotalCornersRepository _repository;

        public TotalCornersService(ITotalCornersRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TotalCorners>> GetAll()
           => await _repository.GetAll();

        public async Task<TotalCorners> GetByIdAsync(string id)
            => await _repository.GetById(id);

        public async Task<TotalCorners> GetByMatchIdAsync(string id, string matchId, string? companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<TotalCorners> InsertAsync(TotalCorners obj)
            => await _repository.Insert(obj);

        public async Task<TotalCorners> UpdateAsync(string id, TotalCorners obj)
            => await _repository.Update(id, obj);

        public async Task<bool> RemoveAsync(string id)
           => await _repository.Remove(id);
    }
}
