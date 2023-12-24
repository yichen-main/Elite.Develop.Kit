using Global = Elite.IIoT.Core.Architects.Elementors.GlobalExtension;

namespace Elite.IIoT.Platform.Facilities.Foundations;
internal sealed class StationBuilder
{
    internal sealed class ServerDirector
    {
        public static async ValueTask BuildAsync<T>(HostBuilder builder) where T : IIoTModule
        {
            (await builder.BuildNativeAsync<T>()).BuildConfigure().BuildRegistration().BuildPipeline();
        }
    }
    internal abstract class HostBuilder
    {
        public abstract HostBuilder BuildConfigure();
        public abstract HostBuilder BuildRegistration();
        public abstract HostBuilder BuildPipeline();
        public abstract ValueTask<HostBuilder> BuildNativeAsync<T>() where T : IIoTModule;
        public abstract WebApplication? GetWebBattleship();
    }
    internal class DefaultBuilder(in string[]? args, Action<IEndpointRouteBuilder>? action) : HostBuilder
    {
        public override async ValueTask<HostBuilder> BuildNativeAsync<T>()
        {
            WebBuilder.Services.ReplaceConfiguration(WebBuilder.Configuration);
            StaticWebAssetsLoader.UseStaticWebAssets(WebBuilder.Environment, WebBuilder.Configuration);
            if (Directory.CreateDirectory(PlugInPath).Exists) await WebBuilder.Services.AddApplicationAsync<T>(options =>
            {
                options.PlugInSources.AddFolder(PlugInPath);
            }).ConfigureAwait(false);
            Profile!.Window.TitleName.OutputKanban(
                Profile.Window.KanbanName,
                Profile.Window.StartingPoint,
                Profile.Window.WordSpacing,
                Profile.Window.DividerLength);
            return this;
        }
        public override HostBuilder BuildConfigure()
        {
            WebBuilder.Host.ConfigureHostOptions(item =>
            {
                item.ServicesStartConcurrently = false;
                item.ServicesStopConcurrently = false;
                item.ShutdownTimeout = TimeSpan.FromSeconds(15);
            }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog((context, provider, config) =>
            {
                config.BuildLogger(provider);
                var initialDefinition = provider.GetService<IInitialDefinition>();
                ArgumentNullException.ThrowIfNull(initialDefinition, nameof(IInitialDefinition));
                initialDefinition.DefineTokenContent(new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "Issuer",
                    ValidateAudience = true,
                    ValidAudience = "Audience",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("baseStation.OptionJWT.SecretKey")),
                });
            });
            return this;
        }
        public override HostBuilder BuildRegistration()
        {
            WebBuilder.WebHost.UseKestrel(item =>
            {
                if (Profile!.Protocol.HTTPS.Enabled) item.ListenAnyIP(Profile.Protocol.HTTPS.Port, config =>
                {
                    config.UseHttps(new X509Certificate2(File.ReadAllBytes(
                        Profile.Protocol.HTTPS.Certificate.Location), Profile.Protocol.HTTPS.Certificate.Password));
                });
                item.ListenAnyIP(Profile.Protocol.HTTP.Port);
                item.ListenAnyIP(Profile.Protocol.MQTT.Port, options => options.UseMqtt());
            });
            return this;
        }
        public override HostBuilder BuildPipeline()
        {
            action += item => item.MapMqtt($"/{Profile!.Protocol.RootPath}.mqtt");
            {
                WebApp = WebBuilder.Build();
                WebApp.UseRouting();
                WebApp.UseCors();
                WebApp.UseAuthentication();
                WebApp.UseAuthorization();
                WebApp.MapControllers();
                WebApp.UseRateLimiter();
                WebApp.UseSwaggerUI(item => item.RoutePrefix = "api/document");
                WebApp.UseSwagger();
                WebApp.UseEndpoints(action);
                WebApp.UseMqttServer(item => WebApp.Services.GetRequiredService<IMQTTService>().Find(item));
            }
            return this;
        }
        public override WebApplication? GetWebBattleship() => WebApp;
        internal WebApplication? WebApp { get; set; }
        WebApplicationBuilder WebBuilder { get; } = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = Global.RootPosition,
        });
    }
    internal static YamlProfile? Profile { get; set; }
    internal static string PlugInPath => Path.Combine(Global.RootPosition, "Modules");
}