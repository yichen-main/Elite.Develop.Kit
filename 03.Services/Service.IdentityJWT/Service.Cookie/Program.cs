using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (HttpContext context) =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    context.Response.Cookies.Append("","");
    //https://blog.csdn.net/HerryDong/article/details/124292744


    //Response.Cookies["Email"].Value = "ddd";
    //Response.Cookies["Email"].Expires = DateTime.Now.AddDays(1);
    return forecast;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

//https://alimozdemir.com/posts/aspnet-core-jwt-and-refresh-token-with-httponly-cookies/



//HttpCookie cookie = new HttpCookie("Test");
//cookie.Expires = DateTime.Now.AddDays(-1);
//HttpContext.Response.Cookies.Add(cookie);