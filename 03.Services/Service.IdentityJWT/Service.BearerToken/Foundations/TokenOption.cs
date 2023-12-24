namespace Service.BearerToken.Foundations;
public sealed class TokenOption
{
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int ExpireMinutes { get; init; }
}