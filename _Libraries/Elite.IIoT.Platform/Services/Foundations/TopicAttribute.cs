namespace Elite.IIoT.Platform.Services.Foundations;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class TopicAttribute : Attribute
{
    public TopicAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
    public string Name { get; init; }
}