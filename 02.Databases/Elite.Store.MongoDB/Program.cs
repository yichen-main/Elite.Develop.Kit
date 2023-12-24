try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<BookStoreDatabaseSettings>(builder.Configuration.GetSection("BookStoreDatabase"));

    builder.Services.AddSingleton<BooksService>();

    builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}