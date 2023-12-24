try
{
    //裝飾器可以理解成"洋蔥模型" => 代替繼承
    //不使用繼承，而以組裝方式動態地為物件加入新功能
    //https://cloud.tencent.com/developer/article/1998631
    var builder = Host.CreateDefaultBuilder(args);

    builder.ConfigureServices(services =>
    {
        services.AddApplicationAsync<RootModule>(options =>
        {
            options.Services.ReplaceConfiguration(services.GetConfiguration());
        });
    }).AddAppSettingsSecretsJson().UseAutofac().UseConsoleLifetime();

    var host = builder.Build();
    {
        //飲料混和
        //host.Services.GetRequiredService<ILoginProcess>().Create();

        //風味咖啡 
        host.Services.GetRequiredService<IFlavoredCoffee>().Create();
    }
    await host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>().InitializeAsync(host.Services);
    await host.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}