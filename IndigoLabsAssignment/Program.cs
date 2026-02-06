using System.Reflection;
using dotenv.net;
using IndigoLabsAssignment.Models;
using IndigoLabsAssignment.Services;
using IndigoLabsAssignment.Services.Cache;
using IndigoLabsAssignment.Services.Cache.Interfaces;
using IndigoLabsAssignment.Services.Interfaces;
using Microsoft.OpenApi;

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

    // API key support in Swagger (X-Api-Key header)
    options.AddSecurityDefinition(
        "ApiKey",
        new OpenApiSecurityScheme
        {
            Description = "API Key needed to access the endpoints.",
            Name = "X-Api-Key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = [],
    });
});

builder.Services.AddMemoryCache();

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

app.Use(
    async (context, next) =>
    {
        const string headerName = "X-Api-Key";

        if (!context.Request.Headers.TryGetValue(headerName, out var providedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API key is missing.");
            return;
        }

        var config = context.RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = config["ApiKeyAuth:Key"];

        if (string.Equals(expectedKey, "super-secret-key-change-me", StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(
                "API key is not configured. Please set a valid key in configuration or environment variables."
            );
            return;
        }

        if (
            string.IsNullOrEmpty(expectedKey)
            || !string.Equals(providedKey, expectedKey, StringComparison.Ordinal)
        )
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key.");
            return;
        }

        await next();
    }
);

app.UseAuthorization();

app.MapControllers();

app.Run();
