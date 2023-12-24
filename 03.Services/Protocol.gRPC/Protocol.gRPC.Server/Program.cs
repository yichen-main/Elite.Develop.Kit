try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseKestrel(item =>
    {
        item.ListenAnyIP(5432, options => options.Protocols = HttpProtocols.Http2);
    });
    builder.Services.AddGrpc();

    var app = builder.Build();
    app.MapGrpcService<EmployeeService>();
    await app.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}