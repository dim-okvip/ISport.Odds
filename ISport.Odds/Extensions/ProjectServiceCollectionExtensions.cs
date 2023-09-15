using Microsoft.Extensions.Options;

namespace ISport.Odds.Extensions
{
    internal static class ProjectServiceCollectionExtensions
    {
        internal static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration) =>
           services
                .AddCors()
                .AddDbContext(configuration)
                .AddRepositoryAndServices()
                //.AddHostedServices()
            ;

        internal static IServiceCollection AddCors(this IServiceCollection services) =>
            services
                .AddCors(options => {
                    options.AddPolicy(Utils.CORS_POLICY, builder => builder
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed((host) => true)
                                .AllowCredentials());
                });

        internal static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
            services
                .Configure<MongoDBSettings>(configuration.GetSection("OddsDatabase"))
                .AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDBSettings>>().Value)
                .AddScoped<MongoContext>();

        internal static IServiceCollection AddRepositoryAndServices(this IServiceCollection services) =>
            services
                .AddScoped<IPreMatchAndInPlayOddsMainRepository, PreMatchAndInPlayOddsMainRepository>()
                .AddScoped<ITotalCornersRepository, TotalCornersRepository>()
                .AddScoped<IPreMatchAndInPlayOddsMainService, PreMatchAndInPlayOddsMainService>()
                .AddScoped<ITotalCornersService, TotalCornersService>()
            ;

        internal static IServiceCollection AddHostedServices(this IServiceCollection services) =>
            services.AddHostedService<SyncFromISportJob>();
    }
}
