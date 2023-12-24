try
{
    var builder = WebApplication.CreateBuilder(args);
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
    {
        builder.WebHost.UseKestrel(item => item.ListenAnyIP(7070));
        builder.Services.Configure<TokenOption>(builder.Configuration.GetSection(nameof(TokenOption)));


        var tokenOption = builder.Services.GetConfiguration().GetSection(nameof(TokenOption)).Get<TokenOption>();
        ArgumentNullException.ThrowIfNull(tokenOption, nameof(TokenOption));
        TokenValidationParameters tokenValidationParams = new()
        {
            //驗證 Issuer
            ValidateIssuer = false,
            ValidIssuer = tokenOption.Issuer, //不驗證就不需要填寫

            //驗證 Audience
            ValidateAudience = false,
            ValidAudience = tokenOption.Audience, //不驗證就不需要填寫

            //驗證 Token 的有效期間
            ValidateLifetime = true,

            //如果 Token 中包含 key 才需要驗證，一般都只有簽章
            //使用我們在 appsettings 中的 SecretKey 來驗證 JWT 的第三部分，並驗證 JWT token 是由我們產生的
            ValidateIssuerSigningKey = true,

            //是不是有過期時間
            RequireExpirationTime = true,

            //時間偏移
            ClockSkew = TimeSpan.Zero,

            //添加密鑰到 JWT 的加密演算法中
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey)),

            //透過這項宣告, 就可以從 "sub" 取值並設定給 User.Identity.Name
            NameClaimType = ClaimTypes.NameIdentifier,

            //透過這項宣告, 就可以從 "roles" 取值, 並可讓 [Authorize] 判斷角色
            RoleClaimType = ClaimTypes.Role
        };

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = tokenValidationParams;

            //當驗證失敗時, 回應標頭會包含 WWW-Authenticate 標頭, 這裡會顯示失敗的詳細錯誤原因
            options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉
        }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {//https://www.cnblogs.com/CreateMyself/p/15755657.html

            //https://kerwenzhang.github.io/web/2021/12/02/JWT/
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.Cookie.Name = "user-session";
            options.SlidingExpiration = true;
        }).AddScheme<UserAuthHandler.Option, UserAuthHandler>(nameof(System), configureOptions: item =>
        {
            item.Realm = "Demo Site";
        });
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
        });
        await builder.AddApplicationAsync<AppModule>();
    }
    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    {
        //登入並取得 JWT
        app.MapPost("/signin", (LoginViewModel login, IVerifyHelper verifyHelper) =>
        {
            if (ValidateUser(login))
            {
                var token = verifyHelper.GenerateAccessToken(login.Username);
                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest();
            }
            static bool ValidateUser(LoginViewModel login)
            {
                return true;
            }
        }).WithName("SignIn").AllowAnonymous()/*允許匿名*/;

        //取得 JWT 中的所有 Claims
        app.MapGet("/claims", [Authorize(AuthenticationSchemes = nameof(System))] (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Claims.Select(item => new { item.Type, item.Value }));
        }).WithName("Claims").RequireAuthorization()/*需要授權*/;

        //取得 JWT 中的使用者名稱
        app.MapGet("/username", (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Identity?.Name);
        }).WithName("Username").RequireAuthorization()/*需要授權*/;

        //取得使用者是否擁有特定角色
        app.MapGet("/isInRole", (ClaimsPrincipal user, string name) =>
        {
            return Results.Ok(user.IsInRole(name));
        }).WithName("IsInRole").RequireAuthorization()/*需要授權*/;

        //取得 JWT 中的 JWT ID
        app.MapGet("/jwtid", (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Claims.FirstOrDefault(item => item.Type is JwtRegisteredClaimNames.Jti)?.Value);
        }).WithName("JwtId").RequireAuthorization()/*需要授權*/;
    }
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
record LoginViewModel(string Username, string Password);