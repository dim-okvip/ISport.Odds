using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.RegularExpressions;

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

        public async Task SendMessage(string? matchId, string? companyId)
        {
            //if (InMemory.ConnectionByMatch.ContainsKey(matchId))
            //    InMemory.ConnectionByMatch[matchId].Add(Context.ConnectionId);
            //else
            //    InMemory.ConnectionByMatch.Add(matchId, new List<string> { Context.ConnectionId });

            await Groups.AddToGroupAsync(Context.ConnectionId, matchId);

            PreMatchAndInPlayOddsMain preMatchAndInPlayOddsMain = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(Source.InMemory, Utils.PreMatchAndInPlayOddsMainId, matchId, companyId);
            TotalCorners totalCornersPreMatch = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersPreMatchId, matchId, companyId);
            TotalCorners totalCornersInPlay = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersInPlayId, matchId, companyId);

            AggregatedOdds aggregatedOdds = new() { PreMatchAndInPlayOddsMain = preMatchAndInPlayOddsMain, TotalCornersPreMatch = totalCornersPreMatch , TotalCornersInPlay = totalCornersInPlay };
            await Clients.Caller.SendAsync("ReceiveMessage", aggregatedOdds);
        }

        public override Task OnConnectedAsync()
        {
            //Console.WriteLine($"Client {Context.ConnectionId} connected at {DateTime.Now}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //Console.WriteLine($"Client {Context.ConnectionId} disconnected at {DateTime.Now}");

            //foreach (KeyValuePair<string, List<string>> entry in InMemory.ConnectionByMatch)
            //{
            //    string? connectionId = entry.Value.Where(x => x == Context.ConnectionId).FirstOrDefault();
            //    if (!String.IsNullOrEmpty(connectionId))
            //        entry.Value.Remove(connectionId);
            //}
            return base.OnDisconnectedAsync(exception);
        }
    }
}
