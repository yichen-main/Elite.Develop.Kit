try
{
    using var application = AbpApplicationFactory.Create<AppModule>(options =>
    {
        options.UseAutofac();
    });
    await application.InitializeAsync();

    var service = application.ServiceProvider.GetService<INpgsqlExpert>();
    if (service is not null) await service.RunAsync();

    Console.WriteLine("Press ENTER to stop application...");
    Console.ReadLine();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}