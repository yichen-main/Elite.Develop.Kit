namespace Elite.IIoT.Platform.Services.Foundations;
internal abstract class DurableCarrier
{
    protected static string GetString(in ArraySegment<byte> bytes) => Encoding.UTF8.GetString(bytes);
    protected static string GetTopic<T>() => typeof(T).GetCustomAttribute<TopicAttribute>()!.Name;
    protected static MqttServer? Server { get; set; } 
}