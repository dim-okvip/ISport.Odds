using ISport.Odds.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ISport.Odds.Services
{
    public class OddsService
    {
        private readonly IMongoCollection<Models.Odds> _oddsCollection;

        public OddsService(IOptions<DatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);

            _oddsCollection = mongoDatabase.GetCollection<Models.Odds>(bookStoreDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<Models.Odds>> GetAsync() =>
            await _oddsCollection.Find(_ => true).ToListAsync();

        public async Task<Models.Odds?> GetAsync(string id) =>
            await _oddsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Models.Odds?> GetByMatchIdAsync(string id, string matchId = "", string companyId = "")
        {
            Models.Odds odds = await _oddsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (odds is not null)
            {
                odds.Data.Handicap = FilterList(odds.Data.Handicap, matchId, companyId);
                odds.Data.EuropeOdds = FilterList(odds.Data.EuropeOdds, matchId, companyId);
                odds.Data.OverUnder = FilterList(odds.Data.OverUnder, matchId, companyId);
                odds.Data.HandicapHalf = FilterList(odds.Data.HandicapHalf, matchId, companyId);
                odds.Data.OverUnderHalf = FilterList(odds.Data.OverUnderHalf, matchId, companyId);
                return odds;
            }
            return new Models.Odds();
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

        public async Task CreateAsync(Models.Odds newOdds) =>
            await _oddsCollection.InsertOneAsync(newOdds);

        public async Task UpdateAsync(string id, Models.Odds updatedOdds) =>
            await _oddsCollection.ReplaceOneAsync(x => x.Id == id, updatedOdds);

        public async Task RemoveAsync(string id) =>
            await _oddsCollection.DeleteOneAsync(x => x.Id == id);
    }
}
