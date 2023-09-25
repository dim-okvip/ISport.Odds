using ISport.Odds.Models;
using Microsoft.AspNetCore.SignalR;

namespace ISport.Odds.Services
{
    public class SyncFromISportJob : BackgroundService
    {
        private Timer _timerPreMatchAndInPlayOdds;
        private Timer _timerTotalCorners;
        private readonly int _intervalPreMatchAndInPlayOdds;
        private readonly int _intervalTotalCorners;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SyncFromISportJob> _logger;
        private readonly Dictionary<string, string> _query;
        private readonly string _urlPreMatchAndInPlayOddsMain;
        private readonly string _urlTotalCornersPreMatch;
        private readonly string _urlTotalCornersInPlay;
        private readonly IHubContext<OddsHub> _hubContext;
        private const int RETAIN_DAYS = 2;

        public SyncFromISportJob(
            IConfiguration configuration, 
            IServiceProvider serviceProvider,
            ILogger<SyncFromISportJob> logger,
            IHubContext<OddsHub> hubContext)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;

            _query = new Dictionary<string, string>(){ ["api_key"] = _configuration["ISport:APIKey"], };
            _urlPreMatchAndInPlayOddsMain = Utils.GetUriWithQueryString(_configuration["ISport:PreMatchAndInPlayOdds.Main"], _query);
            _urlTotalCornersPreMatch = Utils.GetUriWithQueryString(_configuration["ISport:TotalCorners.PreMatch"], _query);
            _urlTotalCornersInPlay = Utils.GetUriWithQueryString(_configuration["ISport:TotalCorners.InPlay"], _query);

            _intervalPreMatchAndInPlayOdds = int.Parse(_configuration["ISport:PreMatchAndInPlayOdds.Interval"]);
            _intervalTotalCorners = int.Parse(_configuration["ISport:TotalCorners.Interval"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timerPreMatchAndInPlayOdds = new Timer(SyncPreMatchAndInPlayOdds, null, TimeSpan.Zero, TimeSpan.FromSeconds(_intervalPreMatchAndInPlayOdds));
            _timerTotalCorners = new Timer(SyncTotalCorners, null, TimeSpan.Zero, TimeSpan.FromSeconds(_intervalTotalCorners));
        }

        private void SyncPreMatchAndInPlayOdds(object? state)
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {

                    IPreMatchAndInPlayOddsMainService _preMatchAndInPlayOddsMainService = scope.ServiceProvider.GetRequiredService<IPreMatchAndInPlayOddsMainService>();
                    string result = Utils.SendGet(_urlPreMatchAndInPlayOddsMain);
                    PreMatchAndInPlayOddsMain? oddsISport = JsonSerializer.Deserialize<PreMatchAndInPlayOddsMain>(result, Utils.JsonSerializerOptions);
                    if (oddsISport is not null && oddsISport.Code is 0)
                    {
                        oddsISport.Id = Utils.PreMatchAndInPlayOddsMainId;
                        PreMatchAndInPlayOddsMain oddsMongoDB = _preMatchAndInPlayOddsMainService.GetByIdAsync(oddsISport.Id).Result;
                        if (oddsMongoDB is null)
                            _preMatchAndInPlayOddsMainService.InsertAsync(oddsISport).Wait();
                        else
                            _preMatchAndInPlayOddsMainService.UpdateAsync(oddsISport.Id, oddsISport).Wait();

                        InMemory.PreMatchAndInPlayOddsMain = oddsISport;

                        _logger.LogInformation($"{DateTime.Now} Synchronization pre-match and in-play odds (main) data from ISport Odds API every {_intervalPreMatchAndInPlayOdds} second succeed!");
                    }
                    else
                        _logger.LogInformation($"{DateTime.Now} ISport API return empty pre-match and in-play odds (main) data!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void SyncTotalCorners(object? state)
        {
            try
            {
                string resultPreMatch = Utils.SendGet(_urlTotalCornersPreMatch);
                string resultInPlay = Utils.SendGet(_urlTotalCornersInPlay);

                TotalCorners? cornerPreMatchISport = JsonSerializer.Deserialize<TotalCorners>(resultPreMatch, Utils.JsonSerializerOptions);
                if (cornerPreMatchISport is not null && cornerPreMatchISport.Code is 0)
                {
                    LoadCornersToDatabaseAndMemory(cornerPreMatchISport, Utils.TotalCornersPreMatchId);
                    _logger.LogInformation($"{DateTime.Now} Synchronization total corners (pre-match) data from ISport Odds API every {_intervalTotalCorners} second succeed!");
                }
                else
                    _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (pre-match) data!");

                TotalCorners? cornerInPlayISport = JsonSerializer.Deserialize<TotalCorners>(resultInPlay, Utils.JsonSerializerOptions);
                if (cornerInPlayISport is not null && cornerInPlayISport.Code is 0)
                {
                    LoadCornersToDatabaseAndMemory(cornerInPlayISport, Utils.TotalCornersInPlayId);
                    _logger.LogInformation($"{DateTime.Now} Synchronization total corners (in-play) data from ISport Odds API every {_intervalTotalCorners} second succeed!");
                }
                else
                    _logger.LogInformation($"{DateTime.Now} ISport API return empty total corners (in-play) data!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void LoadCornersToDatabaseAndMemory(TotalCorners? cornerISport, string totalCornersId)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                ITotalCornersService totalCornersService = scope.ServiceProvider.GetRequiredService<ITotalCornersService>();
                TotalCorners cornerMongoDB = totalCornersService.GetByIdAsync(totalCornersId).Result;
                if (cornerMongoDB is null)
                {
                    cornerISport.Id = totalCornersId;
                    totalCornersService.InsertAsync(cornerISport).Wait();
                }
                else
                {
                    Dictionary<string, List<TotalCornersDetail>> totalCornerDetailChanges = new();
                    foreach (var itemISport in cornerISport.Data)
                    {
                        DateTime datetimeMatch = new DateTime(1970, 01, 01, 7, 0, 0) + TimeSpan.FromSeconds(itemISport.ChangeTime);
                        TimeSpan elaspedTime = DateTime.Now.Subtract(datetimeMatch);
                        if (elaspedTime.TotalDays < RETAIN_DAYS &&
                            !cornerMongoDB.Data.Any(itemMongoDB => itemMongoDB.MatchId == itemISport.MatchId && itemMongoDB.CompanyId == itemISport.CompanyId))
                        {
                            cornerMongoDB.Data.Add(itemISport);
                        }
                    }

                    for (int i = 0; i < cornerMongoDB.Data.Count; i++)
                    {
                        var itemMongoDB = cornerMongoDB.Data[i];

                        DateTime datetimeMatch = new DateTime(1970, 01, 01, 7, 0, 0) + TimeSpan.FromSeconds(itemMongoDB.ChangeTime);
                        TimeSpan elaspedTime = DateTime.Now.Subtract(datetimeMatch);
                        if (elaspedTime.TotalDays >= RETAIN_DAYS)
                        {
                            cornerMongoDB.Data.RemoveAt(i);
                            continue;
                        }

                        foreach (var itemISport in cornerISport.Data)
                        {
                            if (itemMongoDB.MatchId == itemISport.MatchId && itemMongoDB.CompanyId == itemISport.CompanyId)
                            {
                                TotalCornersOddsDetail oddsMongoDB = itemMongoDB.Odds;
                                TotalCornersOddsDetail oddsISport = itemISport.Odds;
                                if (oddsMongoDB.TotalCorners != oddsISport.TotalCorners ||
                                    oddsMongoDB.Over != oddsISport.Over ||
                                    oddsMongoDB.Under != oddsISport.Under)
                                {
                                    itemMongoDB.Odds = oddsISport;
                                    if (totalCornerDetailChanges.ContainsKey(itemMongoDB.MatchId))
                                        totalCornerDetailChanges[itemMongoDB.MatchId].Add(itemISport);
                                    else
                                        totalCornerDetailChanges.Add(itemMongoDB.MatchId, new List<TotalCornersDetail>() { itemISport });
                                }
                            }
                        }
                    }

                    totalCornersService.UpdateAsync(totalCornersId, cornerMongoDB).Wait();

                    string clientMethod = String.Empty;
                    if (totalCornersId == Utils.TotalCornersPreMatchId)
                    {
                        InMemory.TotalCornersPreMatch = cornerMongoDB ?? cornerISport;
                        clientMethod = "ReceiveCornerPreMatchChanges";
                    }

                    if (totalCornersId == Utils.TotalCornersInPlayId)
                    {
                        InMemory.TotalCornersInPlay = cornerMongoDB ?? cornerISport;
                        clientMethod = "ReceiveCornerInPlayChanges";
                    }

                    if (totalCornerDetailChanges.Count > 0)
                    {
                        foreach (var item in totalCornerDetailChanges)
                        {
                            _hubContext.Clients.Group(item.Key).SendAsync(clientMethod, item.Value);
                        }
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
        }
    }
}
