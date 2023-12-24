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

        //���v���A���a�}
        item.AuthorizationEndpoint = "https://localhost:7000/oauth/authorize";

        //��� Token �a�}
        item.TokenEndpoint = "https://localhost:7000/oauth/token";

        item.CallbackPath = "/auth/callback";

        //��[�w�����Ҧ�
        item.UsePkce = true;

        //���� Token �ɷ|Ĳ�o����k
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