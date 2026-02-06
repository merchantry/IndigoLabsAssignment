using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services;
using IndigoLabsAssignment.Services.Interfaces;
using IndigoLabsAssignment.Services.Cache;
using IndigoLabsAssignment.Services.Cache.Interfaces;
using System.Reflection;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddMemoryCache();
// Register file temperature services
builder.Services.AddSingleton<ILineParser, LineParser>();
builder.Services.AddSingleton<ICityTemperatureStatsService, CityTemperatureStatsService>();
builder.Services.AddSingleton<IFileReaderService, FileReaderService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ICityAggregateCacheService, CityAggregateCacheService>();
builder.Services.AddSingleton<IFileMetaDataService, FileMetaDataService>();

// Bind FileSettings
builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IndigoLabsAssignment v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

