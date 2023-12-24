namespace Elite.IIoT.Core.Architects.Elementors;
public static class GlobalExtension
{
    public static void PrintConsole(this string content, in ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(content);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? sources) => sources ?? Enumerable.Empty<T>();
    public static T? ToObject<T>(this string content) => JsonSerializer.Deserialize<T>(content, JsonOption);
    public static T? ToObject<T>(this byte[] contents) => JsonSerializer.Deserialize<T>(contents, JsonOption);
    public static string ToJson<T>(this T @object) => JsonSerializer.Serialize(@object, typeof(T), JsonOption);
    public static string DateTimeFormat { get; set; } = "yyyy/MM/dd HH:mm:ss";
    public static string DefaultLanguage { get; set; } = "en-US";
    public static string SystemFlag { get; set; } = string.Empty;
    public static string RootPosition => Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
    static JsonSerializerOptions JsonOption => new()
    {
        MaxDepth = 100,
        WriteIndented = true,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new DateTimeConverter() },
    };
    class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //實現反序列化邏輯
            if (reader.TokenType is JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }
            return reader.GetDateTime();
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            //實現序列化邏輯
            writer.WriteStringValue(value.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}