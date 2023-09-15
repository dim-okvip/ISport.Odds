namespace ISport.Odds.Services
{
    public class SyncFromISportJob : BackgroundService
    {
        private Timer _timer;
        private readonly int _interval;
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
            _interval = int.Parse(_configuration["ISport:Interval"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));
        }

        private async void DoWork(object? state)
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var getOddsTypeTask = Task.Run(async () =>
                    {
                        IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService = scope.ServiceProvider.GetRequiredService<IPreMatchAndInPlayOddsMainService>();
                        string result = Utils.SendGet(_urlPreMatchAndInPlayOddsMain);
                        PreMatchAndInPlayOddsMain? oddsISport = JsonSerializer.Deserialize<PreMatchAndInPlayOddsMain>(result, Utils.JsonSerializerOptions);
                        if (oddsISport is not null && oddsISport.Code is 0)
                        {
                            oddsISport.Id = Utils.PreMatchAndInPlayOddsMainId;
                            PreMatchAndInPlayOddsMain oddsMongoDB = await _preMatchAndInPlayOddsMainService.GetByIdAsync(oddsISport.Id);
                            if (oddsMongoDB is null)
                                await _preMatchAndInPlayOddsMainService.InsertAsync(oddsISport);
                            else
                                await _preMatchAndInPlayOddsMainService.UpdateAsync(oddsISport.Id, oddsISport);
                            
                            InMemory.PreMatchAndInPlayOddsMain = oddsISport;

                            _logger.LogInformation($"{DateTime.Now} Synchronization pre-match and in-play odds (main) data from ISport Odds API every {_interval} second succeed!");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty pre-match and in-play odds (main) data!");
                    });

                    var getOddsCornerTask = Task.Run(async () =>
                    {
                        ITotalCornersService _totalCornersService = scope.ServiceProvider.GetRequiredService<ITotalCornersService>();
                        string resultPreMatch = Utils.SendGet(_urlTotalCornersPreMatch);
                        string resultInPlay = Utils.SendGet(_urlTotalCornersInPlay);

                        TotalCorners? cornerPreMatchISport = JsonSerializer.Deserialize<TotalCorners>(resultPreMatch, Utils.JsonSerializerOptions);
                        TotalCorners? cornerInPlayISport = JsonSerializer.Deserialize<TotalCorners>(resultInPlay, Utils.JsonSerializerOptions);

                        if (cornerPreMatchISport is not null && cornerPreMatchISport.Code is 0)
                        {
                            cornerPreMatchISport.Id = Utils.TotalCornersPreMatchId;
                            TotalCorners cornerPreMatchMongoDB = await _totalCornersService.GetByIdAsync(cornerPreMatchISport.Id);
                            if (cornerPreMatchMongoDB is null)
                                await _totalCornersService.InsertAsync(cornerPreMatchISport);
                            else
                                await _totalCornersService.UpdateAsync(cornerPreMatchISport.Id, cornerPreMatchISport);
                            
                            InMemory.TotalCornersPreMatch = cornerPreMatchISport;

                            _logger.LogInformation($"{DateTime.Now} Synchronization total corners (pre-match) data from ISport Odds API every {_interval} second succeed!");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (pre-match) data!");

                        if (cornerInPlayISport is not null && cornerInPlayISport.Code is 0)
                        {
                            cornerInPlayISport.Id = Utils.TotalCornersInPlayId;
                            TotalCorners cornerInPlayMongoDB = await _totalCornersService.GetByIdAsync(cornerInPlayISport.Id);
                            if (cornerInPlayMongoDB is null)
                                await _totalCornersService.InsertAsync(cornerInPlayISport);
                            else
                                await _totalCornersService.UpdateAsync(cornerInPlayISport.Id, cornerInPlayISport);

                            InMemory.TotalCornersInPlay = cornerInPlayISport;

                            _logger.LogInformation($"{DateTime.Now} Synchronization total corners (in-play) data from ISport Odds API every {_interval} second succeed!");
                        }
                        else
                            _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (in-play) data!");
                    });

                    await Task.WhenAll(getOddsTypeTask, getOddsCornerTask);
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
