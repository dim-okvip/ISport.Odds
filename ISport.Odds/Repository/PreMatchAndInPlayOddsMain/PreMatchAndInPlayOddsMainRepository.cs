namespace ISport.Odds.Repository
{
    public class PreMatchAndInPlayOddsMainRepository : MongoRepository<PreMatchAndInPlayOddsMain>, IPreMatchAndInPlayOddsMainRepository
    {
        public PreMatchAndInPlayOddsMainRepository(MongoContext context) : base(context)
        {
        }
    }
}
