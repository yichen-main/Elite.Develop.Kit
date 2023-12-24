namespace Service.BearerToken.Foundations;
internal sealed class UserAuthHandler(
    UrlEncoder urlEncoder, ILoggerFactory loggerFactory, IOptionsMonitor<UserAuthHandler.Option> authenticateOption)
    : AuthenticationHandler<UserAuthHandler.Option>(authenticateOption, loggerFactory, urlEncoder)
{
    // readonly IVerifyOrder _verify = verify;
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (Request.Headers.TryGetValue("Authorization", out var value))
            {
                string? header = value;
                var result = string.Empty;
                if (string.IsNullOrEmpty(header)) throw new Exception();
                if (!header.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)) throw new Exception();
                {
                    result = header[JwtBearerDefaults.AuthenticationScheme.Length..].Trim();
                    if (string.IsNullOrEmpty(result)) throw new Exception();
                }
                //if (DateTime.UtcNow.Subtract(_verify.CreateTime).TotalSeconds < _verify.ExpiresIn)
                //{
                //    if (_verify.Token == result) return Task.FromResult(AuthenticateResult.Success(
                //            new AuthenticationTicket(
                //                new GenericPrincipal(
                //                    new ClaimsIdentity(new[]
                //                    {
                //                        new Claim(ClaimTypes.Name, string.Empty)
                //                    }, result), roles: null), Scheme.Name)));
                //}
                //else _verify.CreateToken();
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }
    public sealed class Option : AuthenticationSchemeOptions
    {
        public string? Realm { get; set; }
    }
}