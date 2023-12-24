namespace PostgreSQL;

[DependsOn(typeof(AbpAutofacModule))]
internal sealed class AppModule : AbpModule
{
    const int _timeout = 20;
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        DefaultConnection = context.Services.GetConfiguration().GetConnectionString(nameof(DefaultConnection));
        context.Services.AddDbContext<TableContext>(options =>
        {
            options.EnableDetailedErrors(detailedErrorsEnabled: true);
            options.EnableSensitiveDataLogging(sensitiveDataLoggingEnabled: true);
            options.UseNpgsql(DefaultConnection, npgsql =>
            {
                npgsql.UseNodaTime();
                npgsql.EnableRetryOnFailure();
                npgsql.CommandTimeout(_timeout);
            });
        });
    }
    string? DefaultConnection { get; set; }
}