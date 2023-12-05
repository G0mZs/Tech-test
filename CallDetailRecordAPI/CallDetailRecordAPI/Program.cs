using CallDetailRecordAPI.Data.Configurations;
using CallDetailRecordAPI.Extensions;
using CallDetailRecordAPI.Swagger;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.GetSection(nameof(CdrDatabaseConfiguration));
builder.Services.Configure<CdrDatabaseConfiguration>(configuration);

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
