using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.GetSection(nameof(CdrDatabaseConfiguration));
builder.Services.Configure<CdrDatabaseConfiguration>(configuration);

builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CdrDatabaseConfiguration>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<CdrDatabaseConfiguration>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.Configure<CdrDatabaseConfiguration>(
    builder.Configuration.GetSection(nameof(CdrDatabaseConfiguration)));

builder.Services.AddServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
