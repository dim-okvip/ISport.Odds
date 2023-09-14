namespace ISport.Odds.Repository
{
    public class TotalCornersRepository : MongoRepository<TotalCorners>, ITotalCornersRepository
    {
        public TotalCornersRepository(MongoContext context) : base(context)
        {
        }
    }
}
