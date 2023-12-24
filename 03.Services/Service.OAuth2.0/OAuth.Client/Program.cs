try
{
    var cookieScheme = "cookie1";
    var authenticationScheme = "custom";
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddAuthentication(cookieScheme).AddCookie(cookieScheme).AddOAuth(authenticationScheme, item =>
    {
        item.SignInScheme = cookieScheme;
        item.ClientId = "elihu001";
        item.ClientSecret = "123";

        //授權伺服器地址
        item.AuthorizationEndpoint = "https://localhost:7000/oauth/authorize";

        //獲取 Token 地址
        item.TokenEndpoint = "https://localhost:7000/oauth/token";

        item.CallbackPath = "/auth/callback";

        //更加安全的模式
        item.UsePkce = true;

        //拿到 Token 時會觸發的方法
        item.Events.OnCreatingTicket = item =>
        {
            return Task.CompletedTask;
        };
    });
    var app = builder.Build();
    app.UseAuthentication();
    app.MapGet("/", (HttpContext context) =>
    {
        return context.User.Claims.Select(item => new
        {
            item.Type,
            item.Value
        });
    });
    app.MapGet("/login", (HttpContext context) =>
    {
        return Results.Challenge(new AuthenticationProperties
        {
            RedirectUri = "https://localhost:7000/"
        }, authenticationSchemes: new List<string>() { authenticationScheme });
    });
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}