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
            //���� Issuer
            ValidateIssuer = false,
            ValidIssuer = tokenOption.Issuer, //�����ҴN���ݭn��g

            //���� Audience
            ValidateAudience = false,
            ValidAudience = tokenOption.Audience, //�����ҴN���ݭn��g

            //���� Token �����Ĵ���
            ValidateLifetime = true,

            //�p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ��
            //�ϥΧڭ̦b appsettings ���� SecretKey ������ JWT ���ĤT�����A������ JWT token �O�ѧڭ̲��ͪ�
            ValidateIssuerSigningKey = true,

            //�O���O���L���ɶ�
            RequireExpirationTime = true,

            //�ɶ�����
            ClockSkew = TimeSpan.Zero,

            //�K�[�K�_�� JWT ���[�K�t��k��
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey)),

            //�z�L�o���ŧi, �N�i�H�q "sub" ���Ȩó]�w�� User.Identity.Name
            NameClaimType = ClaimTypes.NameIdentifier,

            //�z�L�o���ŧi, �N�i�H�q "roles" ����, �åi�� [Authorize] �P�_����
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

            //�����ҥ��Ѯ�, �^�����Y�|�]�t WWW-Authenticate ���Y, �o�̷|��ܥ��Ѫ��Բӿ��~��]
            options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����
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
        //�n�J�è��o JWT
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
        }).WithName("SignIn").AllowAnonymous()/*���\�ΦW*/;

        //���o JWT �����Ҧ� Claims
        app.MapGet("/claims", [Authorize(AuthenticationSchemes = nameof(System))] (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Claims.Select(item => new { item.Type, item.Value }));
        }).WithName("Claims").RequireAuthorization()/*�ݭn���v*/;

        //���o JWT �����ϥΪ̦W��
        app.MapGet("/username", (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Identity?.Name);
        }).WithName("Username").RequireAuthorization()/*�ݭn���v*/;

        //���o�ϥΪ̬O�_�֦��S�w����
        app.MapGet("/isInRole", (ClaimsPrincipal user, string name) =>
        {
            return Results.Ok(user.IsInRole(name));
        }).WithName("IsInRole").RequireAuthorization()/*�ݭn���v*/;

        //���o JWT ���� JWT ID
        app.MapGet("/jwtid", (ClaimsPrincipal user) =>
        {
            return Results.Ok(user.Claims.FirstOrDefault(item => item.Type is JwtRegisteredClaimNames.Jti)?.Value);
        }).WithName("JwtId").RequireAuthorization()/*�ݭn���v*/;
    }
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
record LoginViewModel(string Username, string Password);