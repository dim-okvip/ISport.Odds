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

        public async Task SendInitializationMessage(string? matchId, string? companyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, matchId);

            PreMatchAndInPlayOddsMain preMatchAndInPlayOddsMain = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(Source.InMemory, Utils.PreMatchAndInPlayOddsMainId, matchId, companyId);
            TotalCorners totalCornersPreMatch = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersPreMatchId, matchId, companyId);
            TotalCorners totalCornersInPlay = await _totalCornersService.GetByMatchIdAsync(Source.InMemory, Utils.TotalCornersInPlayId, matchId, companyId);

            AggregatedOdds aggregatedOdds = new() { PreMatchAndInPlayOddsMain = preMatchAndInPlayOddsMain, TotalCornersPreMatch = totalCornersPreMatch , TotalCornersInPlay = totalCornersInPlay };
            await Clients.Caller.SendAsync("ReceiveInitializationMessage", aggregatedOdds);
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
