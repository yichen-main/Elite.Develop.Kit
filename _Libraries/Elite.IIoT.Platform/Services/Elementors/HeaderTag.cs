namespace Elite.IIoT.Platform.Services.Elementors;
public record HeaderTag
{
    [FromHeader(Name = "X-Timezone")] public int TimeZone { get; init; }
    [FromHeader(Name = "Accept-Language")] public string? Language { get; init; }
    [FromHeader(Name = "Date-Format")] public string? DateFormat { get; init; }
}