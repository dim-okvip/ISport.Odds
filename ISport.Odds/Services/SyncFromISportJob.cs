namespace ISport.Odds.Services
{
    public class SyncFromISportJob : BackgroundService
    {
        private Timer _timer;
        private readonly int _second = 60;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SyncFromISportJob> _logger;
        private readonly Dictionary<string, string> _query;
        private readonly string _urlPreMatchAndInPlayOddsMain;
        private readonly string _urlTotalCornersPreMatch;
        private readonly string _urlTotalCornersInPlay;

        public SyncFromISportJob(
            IConfiguration configuration, 
            IServiceProvider serviceProvider,
            ILogger<SyncFromISportJob> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;

            _query = new Dictionary<string, string>(){ ["api_key"] = _configuration["ISport:APIKey"], };
            _urlPreMatchAndInPlayOddsMain = Utils.GetUriWithQueryString(_configuration["ISport:PreMatchAndInPlayOdds.Main"], _query);
            _urlTotalCornersPreMatch = Utils.GetUriWithQueryString(_configuration["ISport:TotalCorners.PreMatch"], _query);
            _urlTotalCornersInPlay = Utils.GetUriWithQueryString(_configuration["ISport:TotalCorners.InPlay"], _query);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_second));
        }

        private async void DoWork(object? state)
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService = scope.ServiceProvider.GetRequiredService<IPreMatchAndInPlayOddsMainService>();
                    ITotalCornersService _totalCornersService = scope.ServiceProvider.GetRequiredService<ITotalCornersService>();

                    await Task.Run(async () =>
                    {
                        string resultPreMatchAndInPlayOddsMain = Utils.SendGet(_urlPreMatchAndInPlayOddsMain);
                        PreMatchAndInPlayOddsMain? preMatchAndInPlayOddsMain = JsonSerializer.Deserialize<PreMatchAndInPlayOddsMain>(resultPreMatchAndInPlayOddsMain, Utils.JsonSerializerOptions);
                        if (preMatchAndInPlayOddsMain is not null && preMatchAndInPlayOddsMain.Code is 0)
                        {
                            preMatchAndInPlayOddsMain.Id = Utils.PreMatchAndInPlayOddsMainId;
                            if (_preMatchAndInPlayOddsMainService.GetByIdAsync(preMatchAndInPlayOddsMain.Id) is null)
                                await _preMatchAndInPlayOddsMainService.InsertAsync(preMatchAndInPlayOddsMain);
                            else
                                await _preMatchAndInPlayOddsMainService.UpdateAsync(preMatchAndInPlayOddsMain.Id, preMatchAndInPlayOddsMain);

                            _logger.LogInformation($"{DateTime.Now} Synchronization pre-match and in-play odds (main) data from ISport Odds API every {_second} second succeed");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty pre-match and in-play odds (main) data!");
                    });

                    await Task.Run(async () =>
                    {
                        string resultTotalCornersPreMatch = Utils.SendGet(_urlTotalCornersPreMatch);
                        string resultTotalCornersInPlay = Utils.SendGet(_urlTotalCornersInPlay);

                        TotalCorners? totalCornersPrematch = JsonSerializer.Deserialize<TotalCorners>(resultTotalCornersPreMatch, Utils.JsonSerializerOptions);
                        TotalCorners? totalCornersInplay = JsonSerializer.Deserialize<TotalCorners>(resultTotalCornersInPlay, Utils.JsonSerializerOptions);

                        if (totalCornersPrematch is not null && totalCornersPrematch.Code is 0)
                        {
                            totalCornersPrematch.Id = Utils.TotalCornersPreMatchId;
                            if (_totalCornersService.GetByIdAsync(totalCornersPrematch.Id) is null)
                                await _totalCornersService.InsertAsync(totalCornersPrematch);
                            else
                                await _totalCornersService.UpdateAsync(totalCornersPrematch.Id, totalCornersPrematch);

                            _logger.LogInformation($"{DateTime.Now} Synchronization total corners (pre-match) data from ISport Odds API every {_second} second succeed");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (pre-match) data!");

                        if (totalCornersInplay is not null && totalCornersInplay.Code is 0)
                        {
                            totalCornersInplay.Id = Utils.TotalCornersInPlayId;
                            if (_totalCornersService.GetByIdAsync(totalCornersInplay.Id) is null)
                                await _totalCornersService.InsertAsync(totalCornersInplay);
                            else
                                await _totalCornersService.UpdateAsync(totalCornersInplay.Id, totalCornersInplay);

                            _logger.LogInformation($"{DateTime.Now} Synchronization total corners (in-play) data from ISport Odds API every {_second} second succeed");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (in-play) data!");
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
        }
    }
}
