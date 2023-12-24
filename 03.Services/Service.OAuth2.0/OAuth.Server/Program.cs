try
{
    var cookieScheme = "cookie2";
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddAuthentication(cookieScheme).AddCookie(cookieScheme, options =>
    {
        //�Τ᥼���v������|
        options.LoginPath = "/login";
    });
    builder.Services.AddAuthorization(options => 
    {
        //�K�[����
        options.AddPolicy("authed", item =>
        {
            item.AddAuthenticationSchemes(cookieScheme)
            .RequireAuthenticatedUser(); //�u�ݭn�q�L�{�ҧY�i
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