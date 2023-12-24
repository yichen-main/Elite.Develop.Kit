try
{
    var builder = WebApplication.CreateBuilder(args);
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<WeatherForecastService>();
    builder.Services.AddMudServices();

    //MediatR
    builder.Services.AddSingleton<IGameTaskManager, GameTaskManager>();
    builder.Services.AddMediatR(item =>
    {
        item.RegisterServicesFromAssemblyContaining<TaskMeditaEntryPoint>();
    });

    var app = builder.Build();
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}