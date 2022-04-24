using Flurl.Http.Configuration;
using Hathor.Api.Services;
using Hathor.Api.Services.TrackRepositoryServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddSingleton<UserAgentService>();
builder.Services.AddScoped<SliderTrackRepositoryService>();
builder.Services.AddScoped<DbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
