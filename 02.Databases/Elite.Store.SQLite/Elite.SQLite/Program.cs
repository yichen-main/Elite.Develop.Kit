using Elite.SQLite;

try
{
    Log.Logger ??= new LoggerConfiguration()
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .Enrich.FromLogContext()
        .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning)
        .MinimumLevel.Override(nameof(Volo), LogEventLevel.Warning)
        .MinimumLevel.Override(nameof(System), LogEventLevel.Warning)
        .WriteTo.Async(item => item.File("Logs/logs.txt"))
        .WriteTo.Async(item => item.Console()).CreateLogger();

    Log.Information("Starting console host.");

    var builder = Host.CreateDefaultBuilder(args);
    builder.ConfigureServices(services =>
    {
        services.AddHostedService<MyConsoleAppHostedService>();
        services.AddApplicationAsync<RootModule>(options =>
        {
            options.Services.ReplaceConfiguration(services.GetConfiguration());
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });
    }).AddAppSettingsSecretsJson().UseAutofac().UseConsoleLifetime().UseSerilog((context, provider, config) =>
    {
        config = config.MinimumLevel.Override(nameof(System), LogEventLevel.Warning)
        .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning)
        .MinimumLevel.Override(nameof(Volo), LogEventLevel.Warning)
        .MinimumLevel.Warning().Enrich.FromLogContext()
        .WriteTo.Async(item => item.File("Logs/logs.txt")
        .WriteTo.Async(item => item.Console()).CreateLogger());
    });

    var host = builder.Build();
    await host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>().InitializeAsync(host.Services);
    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    if (ex is HostAbortedException)
    {
        throw;
    }

    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}