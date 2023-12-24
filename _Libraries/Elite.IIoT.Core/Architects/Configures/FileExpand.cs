namespace Elite.IIoT.Core.Architects.Configures;
internal static class FileExpand
{
    const string ProfileFolderName = "Profiles";
    internal static string GetYamlLocation(this Assembly assembly) =>
        Path.Combine(Path.GetDirectoryName(assembly.Location)!, ProfileFolderName, $"{assembly.GetName().Name!}.yml");
    internal static ref T ReadYaml<T>(this string path, ref T document)
    {
        YamlSource yamlSource = new()
        {
            Path = path,
            Optional = default,
            FileProvider = null,
            ReloadOnChange = default,
        };
        yamlSource.ResolveFileProvider();
        ConfigurationBuilder configurationBuilder = new();
        configurationBuilder.Add(yamlSource);
        configurationBuilder.Build().Bind(document);
        return ref document;
    }
    internal static async ValueTask WriteYamlAsync<T>(this T document, string path, CancellationToken token = default)
    {
        await using var fileStream = File.Create(path);
        var buffers = Encoding.ASCII.GetBytes(new SerializerBuilder().Build().Serialize(document));
        await fileStream.WriteAsync(buffers.AsMemory(default, buffers.Length), token);
    }
    internal static async ValueTask<T> RefreshYamlAsync<T>(this string path)
    {
        var dynamic = Activator.CreateInstance<T>();
        if (!File.Exists(path) && Directory.CreateDirectory(Path.Combine(GlobalExtension.RootPosition, ProfileFolderName)).Exists)
        {
            await dynamic.WriteYamlAsync(path);
        }
        return path.ReadYaml(ref dynamic);
    }
}