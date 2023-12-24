using Global = Elite.IIoT.Core.Architects.Elementors.GlobalExtension;

namespace Elite.IIoT.Platform.Facilities.Elementors;
public static class IIoTHost
{
    public static void OutputKanban(this string titleName, string kanbanName, in int startingPoint, int wordSpacing, in int dividerLength)
    {
        Console.Title = titleName;
        Console.CursorVisible = default;
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        ListMenu(
        [
            ("Host Name", Dns.GetHostName()),
            ("User Name", Environment.UserName),
            (".NET SDK", Environment.Version.ToString()),
            ("Internet", NetworkInterface.GetIsNetworkAvailable().ToString()),
            ("Language Tag", Thread.CurrentThread.CurrentCulture.IetfLanguageTag),
            ("Language Name", Thread.CurrentThread.CurrentCulture.DisplayName),
            ("IPv4 Physical", GetLocalIPv4(NetworkInterfaceType.Ethernet).FirstOrDefault() ?? string.Empty),
            ("IPv4 Wireless", GetLocalIPv4(NetworkInterfaceType.Wireless80211).FirstOrDefault() ?? string.Empty),
            ("OS Version", Environment.OSVersion.ToString()),
        ]).PrintConsole(ConsoleColor.Yellow);
        MergeFlag(
        [
            FiggleFonts.Standard.Render(kanbanName.Aggregate(string.Empty.PadLeft(startingPoint, '\u00A0'),
            (first, second) => string.Concat(first, second, string.Empty.PadLeft(wordSpacing, '\u00A0')))),
            new string('*', dividerLength),
            Environment.NewLine,
        ]).PrintConsole();
        static string ListMenu(in (string title, string content)[] groups)
        {
            List<string> lines = [];
            foreach (var (title, content) in groups) lines.Add($"{title,16}   =>   {content,-10}{Environment.NewLine}");
            return MergeFlag([.. lines]);
        }
        static IEnumerable<string> GetLocalIPv4(NetworkInterfaceType networkInterfaceType)
        {
            List<string> results = [];
            Array.ForEach(NetworkInterface.GetAllNetworkInterfaces(), item =>
            {
                if (item.NetworkInterfaceType == networkInterfaceType && item.OperationalStatus is OperationalStatus.Up)
                {
                    foreach (var info in item.GetIPProperties().UnicastAddresses)
                    {
                        if (info.Address.AddressFamily is AddressFamily.InterNetwork) results.Add(info.Address.ToString());
                    }
                }
            });
            return results;
        }
        static string MergeFlag(in string[] args)
        {
            var length = args.Length;
            DefaultInterpolatedStringHandler handler = new(default, length);
            for (int item = default; item < length; item++) handler.AppendFormatted(args[item]);
            return handler.ToStringAndClear();
        }
    }
    public static async Task<IHost> CreateBackgroundProcessAsync<T>(string[]? args = default) where T : IIoTModule
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ContentRootPath = Global.RootPosition,
        });
        if (Directory.CreateDirectory(StationBuilder.PlugInPath).Exists) await builder.Services.AddApplicationAsync<T>(options =>
        {
            options.PlugInSources.AddFolder(StationBuilder.PlugInPath);
        }).ConfigureAwait(false);
        return builder.Build();
    }
    public static async ValueTask<WebApplication?> CreateWebServerAsync<T>(Action<IEndpointRouteBuilder>? endpoint = default,
        string[]? args = default) where T : IIoTModule
    {
        BuildLogger();
        StationBuilder.DefaultBuilder builder = new(args, endpoint);
        await StationBuilder.ServerDirector.BuildAsync<T>(builder);
        return builder.GetWebBattleship();
    }
    public static void BuildLogger() => Log.Logger = new LoggerConfiguration()
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .Enrich.FromLogContext()
        .MinimumLevel.Override(nameof(Volo), LogEventLevel.Error)
        .MinimumLevel.Override(nameof(System), LogEventLevel.Error)
        .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Error)
        .WriteTo.Async(item => item.Console())
        .WriteTo.Async(item => item.File(
            rollingInterval: LogInterval,
            outputTemplate: DefaultTemplate,
            restrictedToMinimumLevel: LogEventLevel,
            retainedFileCountLimit: LogRetainedCount,
            path: Path.Combine(LogRootLocation, LogFileExtension))).CreateBootstrapLogger();
    public static void BuildLogger(this LoggerConfiguration logger, in IServiceProvider provider) => logger
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .Enrich.FromLogContext()
        .ReadFrom.Services(provider)
        .MinimumLevel.Override(nameof(Volo), LogEventLevel.Error)
        .MinimumLevel.Override(nameof(System), LogEventLevel.Error)
        .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Error)
        .WriteTo.Async(item => item.Console())
        .WriteTo.Async(item => item.File(
            rollingInterval: LogInterval,
            outputTemplate: DefaultTemplate,
            restrictedToMinimumLevel: LogEventLevel,
            retainedFileCountLimit: LogRetainedCount,
            path: Path.Combine(LogRootLocation, LogFileExtension)));
    public static RollingInterval LogInterval { get; set; }
    public static LogEventLevel LogEventLevel { get; set; }
    public static int LogRetainedCount { get; set; } = 10;
    public static string LogFileExtension { get; set; } = ".log";
    public static string LogRootLocation { get; set; } = "./Logs";
    public static string LogMessageTemplate { get; set; } = "[{0}] {1}";
    public static string LogOutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss}] {Message:lj}{Exception}{NewLine}";
    public static string DefaultTemplate => "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}]-[{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public static TokenValidationParameters? TokenContent { get; internal set; }
}