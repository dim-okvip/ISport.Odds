using ISport.Odds;
using ISport.Odds.Models;
using ISport.Odds.Services;

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
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("OddsDatabase"));

builder.Services.AddSignalR();

builder.Services.AddSingleton<OddsService>();
//builder.Services.AddHostedService<TimerJob>();
builder.Services.AddSingleton<TimerControl>();

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
