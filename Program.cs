using CountriesBackend.Models;

var builder = WebApplication.CreateBuilder(args);

//Swagger service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//HTTP Client service
builder.Services.AddScoped(client => new HttpClient { BaseAddress = new Uri("https://restcountries.com/v2/") });

//Cache service
builder.Services.AddResponseCaching();

//CORS service
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:8080", "https://frontendrestcountries.azurewebsites.net",  "http://frontendrestcountries.azurewebsites.net")
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(MyAllowSpecificOrigins);
app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(180)
        };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
        new string[] { "Accept-Encoding" };

    await next();
});

//Default route to show server is online
app.MapGet("/", () => "Server is online.");

//return all countries. Include only name and flags
app.MapGet("countries/all", async (HttpClient http) =>
    {
        using (HttpResponseMessage response = await http.GetAsync("all?fields=name,flags"))
        {
            try
            {
                response.EnsureSuccessStatusCode();
                return Results.Ok(new { Message = await response.Content.ReadFromJsonAsync<List<CountryBasic>>() });
            }
            catch (Exception)
            {
                return Results.BadRequest(new { Message = $"Error fetching data" });
            }
        }
    }
).Produces<CountryBasic>();

//Return a single country with all information
app.MapGet("countries/{name}", async (HttpClient http, string name) =>
    {
        using (HttpResponseMessage response = await http.GetAsync($"name/{name}"))
        {
            try
            {
                response.EnsureSuccessStatusCode();
                return Results.Ok(new { Message = await response.Content.ReadFromJsonAsync<List<Country>>() });
            }
            catch (Exception)
            {
                return Results.BadRequest(new { Message = $"Error fetching country {name}" });
            }
        }
    }
).Produces<Country>();

app.Run();