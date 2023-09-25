using System.Diagnostics;

namespace ISport.Odds.Extensions
{
    internal static class WebApplicationExtension
    {
        internal static WebApplication InitDataInMemory(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                IPreMatchAndInPlayOddsMainService preMatchAndInPlayOddsMainService = scope.ServiceProvider.GetRequiredService<IPreMatchAndInPlayOddsMainService>();
                ITotalCornersService totalCornersService = scope.ServiceProvider.GetRequiredService<ITotalCornersService>();
                ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                InMemory.PreMatchAndInPlayOddsMain = preMatchAndInPlayOddsMainService.GetByIdAsync(Utils.PreMatchAndInPlayOddsMainId).Result ?? new();
                InMemory.TotalCornersPreMatch = totalCornersService.GetByIdAsync(Utils.TotalCornersPreMatchId).Result ?? new();
                InMemory.TotalCornersInPlay = totalCornersService.GetByIdAsync(Utils.TotalCornersInPlayId).Result ?? new();

                logger.LogInformation($"Loading data from MongoDB to in-memory took {stopwatch.ElapsedMilliseconds} ms");
                stopwatch.Stop();
            }
            return app;
        }
    }
}
