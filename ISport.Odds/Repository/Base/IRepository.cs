namespace ISport.Odds.Repository.Base
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> InsertAsync(TEntity obj);
        Task<TEntity> GetByIdAsync(string id);
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> UpdateAsync(string id, TEntity obj);
        Task<bool> RemoveAsync(string id);
    }
}
