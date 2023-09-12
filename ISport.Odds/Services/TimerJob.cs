using System.Text.Json;

namespace ISport.Odds.Services
{
    public class TimerJob : BackgroundService
    {
        private Timer _timer;
        private readonly int _second = 60;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TimerJob> _logger;
        private readonly Dictionary<string, string> _query;
        private readonly OddsService _oddsService;

        public TimerJob(IConfiguration configuration, ILogger<TimerJob> logger, OddsService oddsService)
        {
            _configuration = configuration;
            _logger = logger;
            _query = new Dictionary<string, string>()
            {
                ["api_key"] = _configuration["ISport:APIKey"],
            };
            _oddsService = oddsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background job starts along with api application on start");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_second));
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation($"Query ISport API runs for every {_second} second");

            string url = Utils.GetUriWithQueryString(_configuration["ISport:PreMatchAndInPlayOdds.Main"], _query);
            string result = Utils.SendGet(url);

            Models.Odds? odds = JsonSerializer.Deserialize<Models.Odds>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (odds is not null && odds.Code is 0) {
                odds.Id = Utils.PreMatchAndInPlayOddsMainId;
                if (_oddsService.GetAsync(odds.Id) is null)
                    await _oddsService.CreateAsync(newOdds: odds);
                else 
                    await _oddsService.UpdateAsync(odds.Id, updatedOdds: odds);

                _logger.LogInformation($"Query succeed!");
            }
            else
                _logger.LogInformation($"Query fail!");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background job stops along with api application on stop");
        }
    }
}
