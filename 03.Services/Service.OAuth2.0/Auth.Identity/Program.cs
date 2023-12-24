try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<DataContext>(options => 
    {
        var jj = builder.Configuration.GetConnectionString("DefaultConnection");
        var sqliteDbPath = Path.Combine(builder.Environment.ContentRootPath, "static-files.sqlite");
        options.UseSqlite($"data source={sqliteDbPath}", b => b.MigrationsAssembly("DemoWeb"));
    });

    builder.Services.AddAuthorization();

    builder.Services.AddIdentityApiEndpoints<IdentityUser>()
        .AddEntityFrameworkStores<DataContext>();

    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapIdentityApi<IdentityUser>();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}