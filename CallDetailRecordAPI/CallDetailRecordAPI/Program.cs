using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Extensions;
using CallDetailRecordAPI.Swagger;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;

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

builder.Services.AddServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    //c.SchemaFilter<EnumSchemaFilter>();

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CallDetailRecordAPI", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
