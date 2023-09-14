namespace ISport.Odds.Repository.Base
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> Insert(TEntity obj);
        Task<TEntity> GetById(string id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> Update(string id, TEntity obj);
        Task<bool> Remove(string id);
    }
}
