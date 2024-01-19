using API_Authentication_ApiKey.Middlewares;
using API_Authentication_ApiKey.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// The API Key Authentication Definition for Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Implementing Api Key Authentication", Version = "v1" });
    c.AddSecurityDefinition("APIAuthKey", new OpenApiSecurityScheme
    {
        Description = "The Auth ApiKey must appear in HTTP Request header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-AuthKey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });
    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "APIAuthKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
    c.AddSecurityRequirement(requirement);
});
// Authentication and Authorization Services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddTransient<IApiAuthKeyValidatorService, ApiAuthKeyValidatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Register the Middleware
app.UseApiKeyAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/wf", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
