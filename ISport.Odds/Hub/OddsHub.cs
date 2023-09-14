using Microsoft.AspNetCore.SignalR;

namespace ISport.Odds
{
    public class OddsHub : Hub
    {
        //public static List<string> LisConnectedConnection = new List<string>();
        private readonly IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService;

        public OddsHub(IPreMatchAndInPlayOddsMainService preMatchAndInPlayOddsMainService)
        {
            _preMatchAndInPlayOddsMainService = preMatchAndInPlayOddsMainService;
        }

        public async Task SendMessage(string matchId, string? companyId)
        {
            PreMatchAndInPlayOddsMain? odds = await _preMatchAndInPlayOddsMainService.GetByMatchIdAsync(Utils.PreMatchAndInPlayOddsMainId, matchId, companyId);
            //await Clients.Client(connectionId).SendAsync("ReceiveMessage", odds);
            await Clients.Caller.SendAsync("ReceiveMessage", odds);
        }

        public override Task OnConnectedAsync()
        {
            //LisConnectedConnection.Add(Context.ConnectionId);
            Console.WriteLine($"Client {Context.ConnectionId} connected at {DateTime.Now}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //LisConnectedConnection.Remove(Context.ConnectionId);
            Console.WriteLine($"Client {Context.ConnectionId} disconnected at {DateTime.Now}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
