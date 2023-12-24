try
{
    var cookieScheme = "cookie2";
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddAuthentication(cookieScheme).AddCookie(cookieScheme, options =>
    {
        //用戶未授權跳轉路徑
        options.LoginPath = "/login";
    });
    builder.Services.AddAuthorization(options => 
    {
        //添加策略
        options.AddPolicy("authed", item =>
        {
            item.AddAuthenticationSchemes(cookieScheme)
            .RequireAuthenticatedUser(); //只需要通過認證即可
        });
    });
    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}