using MongoDB.Driver;

namespace ISport.Odds.Repository.Base
{
    public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<TEntity> DbSet;

        protected MongoRepository(MongoContext context)
        {
            Database = context.Database;
            DbSet = Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task<TEntity> GetByIdAsync(string id) 
            => await DbSet.Find(FilterId(id)).SingleOrDefaultAsync();

        public virtual async Task<TEntity> InsertAsync(TEntity obj)
        {
            await DbSet.InsertOneAsync(obj);
            return obj;
        }

        public virtual async Task<TEntity> UpdateAsync(string id, TEntity obj)
        {
            await DbSet.ReplaceOneAsync(FilterId(id), obj);
            return obj;
        }

        public virtual async Task<bool> RemoveAsync(string id)
        {
            var result = await DbSet.DeleteOneAsync(FilterId(id));
            return result.IsAcknowledged;
        }

        public void Dispose()
           => GC.SuppressFinalize(this);

        private static FilterDefinition<TEntity> FilterId(string key) 
            => Builders<TEntity>.Filter.Eq("Id", key);
    }
}
