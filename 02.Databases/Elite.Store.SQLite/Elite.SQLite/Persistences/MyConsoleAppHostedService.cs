namespace Elite.SQLite.Persistences;
public class MyConsoleAppHostedService(
    HelloWorldService helloWorldService, 
    IAbpApplicationWithExternalServiceProvider abpApplication) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await helloWorldService.SayHelloAsync();
    }
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await abpApplication.ShutdownAsync();
    }
}