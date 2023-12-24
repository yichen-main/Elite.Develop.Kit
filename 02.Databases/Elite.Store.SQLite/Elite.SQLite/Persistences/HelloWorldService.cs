namespace Elite.SQLite.Persistences;

public class HelloWorldService : ITransientDependency
{
    public HelloWorldService() => Logger = NullLogger<HelloWorldService>.Instance;
    public Task SayHelloAsync()
    {
        Logger.LogInformation("Hello World!");
        return Task.CompletedTask;
    }
    public ILogger<HelloWorldService> Logger { get; set; }
}