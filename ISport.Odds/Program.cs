using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
const string CORS_POLICY = "CorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(CORS_POLICY, builder => builder
//        .WithOrigins("http://localhost:5500")
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials()
//        .SetIsOriginAllowed((host) => true));
//});

// Add services to the container.

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("OddsDatabase"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddScoped<MongoContext>();

builder.Services.AddScoped<IPreMatchAndInPlayOddsMainRepository, PreMatchAndInPlayOddsMainRepository>();
builder.Services.AddScoped<IPreMatchAndInPlayOddsMainService, PreMatchAndInPlayOddsMainService>();
builder.Services.AddScoped<ITotalCornersRepository, TotalCornersRepository>();
builder.Services.AddScoped<ITotalCornersService, TotalCornersService>();

builder.Services.AddSignalR();

builder.Services.AddHostedService<SyncFromISportJob>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CORS_POLICY);

app.UseAuthorization();

app.MapControllers();

app.MapHub<OddsHub>("/oddsHub");

app.Run();
