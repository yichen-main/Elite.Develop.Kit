using Elite.IIoT.Platform.Facilities.Elementors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MQTTnet.AspNetCore;

try
{
    //await (await IIoTHost.CreateWebServerAsync<RootModule>())!.RunAsync();

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = GlobalExtension.RootPosition,
    });

    builder.Services.ReplaceConfiguration(builder.Configuration);
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
    await builder.Services.AddApplicationAsync<RootModule>().ConfigureAwait(false);

    builder.WebHost.UseKestrel(item =>
    {
        item.ListenAnyIP(7260);
        item.ListenAnyIP(1883, options => options.UseMqtt());
    });

    builder.Host.ConfigureHostOptions(item =>
    {
        item.ServicesStartConcurrently = false;
        item.ServicesStopConcurrently = false;
        item.ShutdownTimeout = TimeSpan.FromSeconds(15);
    }).AddAppSettingsSecretsJson().UseSystemd().UseSerilog((context, provider, config) =>
    {
        config.BuildLogger(provider);
    });

    //builder.Services.AddTransient<MqttServer>();
    builder.Services.AddHostedMqttServer(options =>
    {    
        options.WithoutDefaultEndpoint();
    });
    builder.Services.AddMqttConnectionHandler();
    builder.Services.AddConnections();
    var app = builder.Build();

    app.UseRouting();
    app.UseEndpoints(item => item.MapMqtt("/iiot/service.mqtt"));
    app.UseMqttServer(item =>
    {
        item.InterceptingPublishAsync += @event =>
        {
            return Task.CompletedTask;
        };
    });
    await app.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(IIoTHost.LogMessageTemplate, nameof(Program), new { e.Message, e.StackTrace });
}
finally
{
    Log.CloseAndFlush();
}