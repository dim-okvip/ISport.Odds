using Microsoft.AspNetCore.SignalR;

namespace ISport.Odds
{
    public class OddsHub : Hub
    {
        private readonly IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService;
        private readonly ITotalCornersService _totalCornersService;

        public OddsHub(IPreMatchAndInPlayOddsMainService preMatchAndInPlayOddsMainService, ITotalCornersService totalCornersService)
        {
            _preMatchAndInPlayOddsMainService = preMatchAndInPlayOddsMainService;
            _totalCornersService = totalCornersService;
        }

        public async Task SendInitializationOddsByMatch(string? matchId, string? companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, matchId);

            PreMatchAndInPlayOddsMain preMatchAndInPlayOddsMain = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(Source.InMemory, Utils.PreMatchAndInPlayOddsMainId, matchId, companyId);
            //TotalCorners totalCornersPreMatch = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersPreMatchId, matchId, companyId);
            //TotalCorners totalCornersInPlay = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersInPlayId, matchId, companyId);

            AggregatedOdds aggregatedOdds = new() { PreMatchAndInPlayOddsMain = preMatchAndInPlayOddsMain }; //, TotalCornersPreMatch = totalCornersPreMatch , TotalCornersInPlay = totalCornersInPlay };
            await Clients.Caller.SendAsync("ReceiveInitializationOddsByMatch", aggregatedOdds);
        }

        public async Task SendInitializationOddsAllMatches()
        {
            PreMatchAndInPlayOddsMain preMatchAndInPlayOddsMain = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(Source.InMemory, Utils.PreMatchAndInPlayOddsMainId, null, null);

            Dictionary<string, OddsType> oddsByMatch = new();
            preMatchAndInPlayOddsMain.Data.Handicap.ForEach(item =>
            {
                string matchId = item.Split(",")[0];
                if (oddsByMatch.ContainsKey(matchId))
                    oddsByMatch[matchId].Handicap.Add(item);
                else
                    oddsByMatch.Add(matchId, new OddsType() { Handicap = new List<string>() { item } });
            });
            
            preMatchAndInPlayOddsMain.Data.EuropeOdds.ForEach(item =>
            {
                string matchId = item.Split(",")[0];
                if (oddsByMatch.ContainsKey(matchId))
                    oddsByMatch[matchId].EuropeOdds.Add(item);
                else
                    oddsByMatch.Add(matchId, new OddsType() { EuropeOdds = new List<string>() { item } });
            });
            
            preMatchAndInPlayOddsMain.Data.OverUnder.ForEach(item =>
            {
                string matchId = item.Split(",")[0];
                if (oddsByMatch.ContainsKey(matchId))
                    oddsByMatch[matchId].OverUnder.Add(item);
                else
                    oddsByMatch.Add(matchId, new OddsType() { OverUnder = new List<string>() { item } });
            });
            
            preMatchAndInPlayOddsMain.Data.HandicapHalf.ForEach(item =>
            {
                string matchId = item.Split(",")[0];
                if (oddsByMatch.ContainsKey(matchId))
                    oddsByMatch[matchId].HandicapHalf.Add(item);
                else
                    oddsByMatch.Add(matchId, new OddsType() { HandicapHalf = new List<string>() { item } });
            });
            
            preMatchAndInPlayOddsMain.Data.OverUnderHalf.ForEach(item =>
            {
                string matchId = item.Split(",")[0];
                if (oddsByMatch.ContainsKey(matchId))
                    oddsByMatch[matchId].OverUnderHalf.Add(item);
                else
                    oddsByMatch.Add(matchId, new OddsType() { OverUnderHalf = new List<string>() { item } });
            });

            await Clients.Caller.SendAsync("ReceiveInitializationOddsAllMatches", oddsByMatch);
        }

        public override Task OnConnectedAsync()
        {
            //Console.WriteLine($"Client {Context.ConnectionId} connected at {DateTime.Now}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //Console.WriteLine($"Client {Context.ConnectionId} disconnected at {DateTime.Now}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
