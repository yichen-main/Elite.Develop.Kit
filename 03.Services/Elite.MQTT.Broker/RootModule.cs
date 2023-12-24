using Elite.IIoT.Platform;

namespace Elite.MQTT.Broker;

[DependsOn(typeof(EliteIIoTPlatformModule))]
internal sealed class RootModule : IIoTModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.AddHostedService<CollectionService>();
        //context.Services.AddHostedMqttServer(optionsBuilder =>
        //{
        //    optionsBuilder.WithDefaultEndpoint();
        //});
        //context.Services.AddMqttConnectionHandler();
        //context.Services.AddConnections();
    }
}