using Rely = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Elite.IIoT.Platform.Services.Repositories;
internal interface IUserAuthenticator
{
    string GenerateAccessToken(in Guid userId, in string account);
    ValueTask<string> GenerateRefreshTokenAsync(string accessToken);
}

[Rely(ServiceLifetime.Singleton)]
file sealed class UserAuthenticator : IUserAuthenticator
{
    public string GenerateAccessToken(in Guid userId, in string account)
    {
        var nowTime = DateTimeOffset.UtcNow;
        return TokenHandler.WriteToken(new JwtSecurityToken(
        issuer: "baseStation.Value.OptionJWT.Issuer",
        audience: "baseStation.Value.OptionJWT.Audience",
        claims: new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, account),
            new(JwtRegisteredClaimNames.Jti, userId.ToString()),
            new(JwtRegisteredClaimNames.Iss, "baseStation.Value.OptionJWT.Issuer"),
            new(JwtRegisteredClaimNames.Aud, "baseStation.Value.OptionJWT.Audience"),
            new(JwtRegisteredClaimNames.Nbf, nowTime.ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Iat, nowTime.ToUnixTimeSeconds().ToString()),
           // new(JwtRegisteredClaimNames.Exp, nowTime.AddMinutes("baseStation.Value.OptionJWT.AccessTokenValidityInMinutes").ToUnixTimeSeconds().ToString()),
        },
        notBefore: nowTime.DateTime,
       //   expires: nowTime.DateTime.AddMinutes(baseStation.Value.OptionJWT.AccessTokenValidityInMinutes),
       signingCredentials: new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("baseStation.Value.OptionJWT.SecretKey")), SecurityAlgorithms.HmacSha256)));
    }
    public async ValueTask<string> GenerateRefreshTokenAsync(string accessToken)
    {
        if (!TokenHandler.CanReadToken(accessToken)) throw new Exception("JWT format error");

        // Validation 1 - Validation JWT token format
        var claimsPrincipal = TokenHandler.ValidateToken(accessToken, IIoTHost.TokenContent, out var securityToken);

        // Validation 2 - Validate encryption alg
        if (securityToken is JwtSecurityToken jwtSecurityToken)
        {
            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

            if (!result)
            {
                return null;
            }
        }

        // Validation 3 - validate expiry date
        // 验证原 token 的过期时间，得到 unix 时间戳
        var expiryDate = UnixTimeStampToUTC(claimsPrincipal.Claims.First(item => item.Type is JwtRegisteredClaimNames.Exp).Value);
        if (expiryDate > DateTime.UtcNow)
        {
            //return new AuthResult()
            //{
            //    Success = false,
            //    Errors = new List<string>()
            //    {
            //        "Token has not yet expired"
            //    }
            //};
        }

        try
        {
            // validation 4 - validate existence of the token
            // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
            //var storedRefreshToken = await _apiDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

            //if (storedRefreshToken == null)
            //{
            //    //return new AuthResult()
            //    //{
            //    //    Success = false,
            //    //    Errors = new List<string>()
            //    //{
            //    //    "Refresh Token does not exist"
            //    //}
            //    //};
            //}

            // Validation 5 - 检查存储的 RefreshToken 是否已过期
            // Check the date of the saved refresh token if it has expired
            //if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            //{
            //    return new AuthResult()
            //    {
            //        Errors = new List<string>() { "Refresh Token has expired, user needs to re-login" },
            //        Success = false
            //    };
            //}

            // Validation 6 - validate if used
            // 验证 refresh token 是否已使用
            //if (storedRefreshToken.IsUsed)
            //{
            //    return new AuthResult()
            //    {
            //        Success = false,
            //        Errors = new List<string>()
            //    {
            //        "Refresh Token has been used"
            //    }
            //    };
            //}

            // Validation 7 - validate if revoked
            // 检查 refresh token 是否被撤销
            //if (storedRefreshToken.IsRevorked)
            //{
            //    return new AuthResult()
            //    {
            //        Success = false,
            //        Errors = new List<string>()
            //    {
            //        "Refresh Token has been revoked"
            //    }
            //    };
            //}

            // Validation 8 - validate the id
            // 这里获得原 JWT token Id
            var jti = claimsPrincipal.Claims.First(item => item.Type is JwtRegisteredClaimNames.Jti).Value;

            // 根据数据库中保存的 Id 验证收到的 token 的 Id
            //if (storedRefreshToken.JwtId != jti)
            //{
            //    return new AuthResult()
            //    {
            //        Success = false,
            //        Errors = new List<string>()
            //    {
            //        "The token doesn't mateched the saved token"
            //    }
            //    };
            //}

            // update current token 
            // 将该 refresh token 设置为已使用
            //storedRefreshToken.IsUsed = true;
            //_apiDbContext.RefreshTokens.Update(storedRefreshToken);
            //await _apiDbContext.SaveChangesAsync();

            //// 生成一个新的 token
            //var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            //return await GenerateJwtToken(dbUser);
        }
        catch (SecurityTokenException)
        {
            throw new Exception("传入AccessToken被修改");
        }
        static DateTime UnixTimeStampToUTC(string unixTimeStamp)
        {
            DateTime dateTimeVal = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(long.Parse(unixTimeStamp)).ToUniversalTime();
            return dateTimeVal;
        }
        return null;
    }
    static JwtSecurityTokenHandler TokenHandler => new();
}