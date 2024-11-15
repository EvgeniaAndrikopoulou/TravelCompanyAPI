using AggregatedAPI_TravelCompanion.Configurations;
using AggregatedAPI_TravelCompanion.Services.AggregationService;
using AggregatedAPI_TravelCompanion.Services.LandmarkService;
using AggregatedAPI_TravelCompanion.Services.LocationService;
using AggregatedAPI_TravelCompanion.Services.WeatherSevice;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient<ILocationService, LocationService>();
UtilityConfig.LocationBaseAPI = builder.Configuration["ServiceUrls:LocationInfo"];
UtilityConfig.LocationKeyAPI = builder.Configuration["APIKeys:LocationInfo"];

builder.Services.AddHttpClient<IWeatherService, WeatherService>();
UtilityConfig.WeatherBaseAPI = builder.Configuration["ServiceUrls:WeatherInfo"];
UtilityConfig.WeatherKeyAPI = builder.Configuration["APIKeys:WeatherInfo"];

builder.Services.AddHttpClient<ILandmarkService, LandmarkService>();
UtilityConfig.LandmarkBaseAPI = builder.Configuration["ServiceUrls:FoursquarePlacesAPI"];
UtilityConfig.LandmarkKeyAPI = builder.Configuration["APIKeys:FoursquarePlacesAPI"];

builder.Services.AddScoped<IAggregationService, AggregationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();