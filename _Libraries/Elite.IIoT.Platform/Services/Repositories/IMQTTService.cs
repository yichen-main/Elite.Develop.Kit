using Rely = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.IIoT.Platform.Services.Repositories;
public interface IMQTTService
{
    void Find(in MqttServer server);
    void Subscribe<T>(Action<T> @object);
    Task PublishAsync<T>(object @object, CancellationToken cancellationToken = default);
}

[Rely(ServiceLifetime.Singleton)]
file sealed class MQTTService : DurableCarrier, IMQTTService
{
    public void Find(in MqttServer server) => Server ??= server;
    public void Subscribe<T>(Action<T> @object) => Server!.InterceptingPublishAsync += @event => Task.Run(() =>
    {
        try
        {
            if (string.Equals(GetTopic<T>(), @event.ApplicationMessage.Topic, StringComparison.OrdinalIgnoreCase))
            {
                var value = GetString(@event.ApplicationMessage.PayloadSegment).ToObject<T>();
                if (value is not null) @object(value);
            }
        }
        catch (Exception) { }
        return Task.CompletedTask;
    }, @event.CancellationToken);
    public async Task PublishAsync<T>(object @object, CancellationToken cancellationToken = default)
    {
        try
        {
            MqttApplicationMessage application = new()
            {
                Retain = true,
                Topic = GetTopic<T>(),
                PayloadSegment = Encoding.UTF8.GetBytes(@object.ToJson()),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce,
            };
            await Server!.InjectApplicationMessage(new InjectedMqttApplicationMessage(application)
            {
                SenderClientId = Guid.NewGuid().ToString(),
            }, cancellationToken);
        }
        catch (Exception) { }
    }
}