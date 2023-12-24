try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.MapGet("/", (HttpContext context) =>
    {
        return context.User.Claims.Select(item => new
        {
            item.Type,
            item.Value
        });
    }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    await app.RunAsync();
}
catch(Exception e)
{
    Console.WriteLine(e.ToString());
}