namespace Elite.IIoT.Core.Architects.Elementors;
public abstract class IIoTModule : AbpModule
{
    protected static T? CreateAppSettingsObject<T>(ServiceConfigurationContext context) where T : class
    {
        var type = typeof(T);
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<T>(configuration.GetSection(type.Name));
        return configuration.GetSection(type.Name).Get<T>();
    }
    protected static async ValueTask<T?> CreateProfileAsync<T>(Assembly assembly) where T : class
    {
        var location = assembly.GetYamlLocation();
        var result = await location.RefreshYamlAsync<T>();
        if (result is not null) await result.WriteYamlAsync(location);
        return result;
    }
}