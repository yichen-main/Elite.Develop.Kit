using Global = Elite.IIoT.Core.Architects.Elementors.GlobalExtension;

namespace Elite.IIoT.Platform;

[DependsOn(typeof(EliteIIoTCoreModule))]
public sealed class EliteIIoTPlatformModule : IIoTModule
{
    public EliteIIoTPlatformModule() => AsyncHelper.RunSync(async () =>
    {
        StationBuilder.Profile = await CreateProfileAsync<YamlProfile>(Assembly.GetExecutingAssembly());
    });
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSoapCore();
        context.Services.AddSwaggerGen();
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
            options.Filters.Add<ExceptionFilter>();
        }).ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = default;
            options.InvalidModelStateResponseFactory = context =>
            {
                List<string> results = [.. RefreshMessage()];
                return new UnprocessableEntityObjectResult(new
                { Message = string.Join(",\u00A0", results), })
                { ContentTypes = { MediaTypeNames.Application.Json } };
                IEnumerable<string> RefreshMessage()
                {
                    foreach (var entry in context.ModelState.Root.Children.OrEmptyIfNull())
                    {
                        for (int i = default; i < entry.Errors.Count; i++) yield return entry.Errors[i].ErrorMessage;
                    }
                }
            };
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.DateFormatString = Global.DateTimeFormat;
            options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
        }).AddMvcOptions(options => options.Conventions.Add(new ModelConvention())).AddControllersAsServices();
        context.Services.AddRateLimiter(options => options.AddFixedWindowLimiter(nameof(IIoT), item =>
        {
            item.QueueLimit = 6;
            item.PermitLimit = 10;
            item.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            item.Window = TimeSpan.FromSeconds(1);
        }));
        context.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Events = new()
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                    }
                    return Task.CompletedTask;
                },
            };
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = IIoTHost.TokenContent!;
        }).AddScheme<UserAuthHandler.Option, UserAuthHandler>(nameof(IIoT), configureOptions: options =>
        {
            options.Realm = "Demo Site";
        });
        context.Services.AddAuthorization();
        context.Services.AddCors(options => options.AddDefaultPolicy(item =>
        {
            item.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*");
        }));
        context.Services.AddHostedMqttServer(options => options.WithoutDefaultEndpoint());
        context.Services.AddMqttConnectionHandler();
        context.Services.AddConnections();
    }
}