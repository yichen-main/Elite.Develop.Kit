namespace Service.BearerToken.Repositories;
internal interface IVerifyHelper
{
    string GenerateAccessToken(in string userName);
    ValueTask<string> GenerateRefreshTokenAsync(string accessToken);
    void SetTokenContent(in TokenValidationParameters parameters);
    TokenValidationParameters TokenValidationParameters { get; }
}

[Dependency(ServiceLifetime.Singleton)]
public class VerifyHelper(IOptions<TokenOption> tokenOption) : IVerifyHelper
{
    public string GenerateAccessToken(in string userName)
    {
        //1.定義需要使用到的 Claims
        var claims = new Claim[]
        {
            //User.Identity.Name
            new(JwtRegisteredClaimNames.Sub, userName),
            new(JwtRegisteredClaimNames.Iss, tokenOption.Value.Issuer),
            new(JwtRegisteredClaimNames.Aud, tokenOption.Value.Audience),

            //JWT ID
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 

            //必須為數字
            new(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(tokenOption.Value.ExpireMinutes).ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        //2.從 appsettings.json 中讀取 SecretKey
        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(tokenOption.Value.SecretKey));

        //3.選擇加密算法
        var algorithm = SecurityAlgorithms.HmacSha256;

        //4.生成 Credentials
        SigningCredentials signingCredentials = new(secretKey, algorithm);

        //5.從 appsettings.json 中讀取 Expires
        var expires = Convert.ToDouble(tokenOption.Value.ExpireMinutes);

        //6.根據以上，生成 Token
        JwtSecurityToken token = new(
            issuer: tokenOption.Value.Issuer,
            audience: tokenOption.Value.Audience, //通常不驗證
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expires),
            signingCredentials: signingCredentials
        );

        // 7.將 Token 變為 String
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async ValueTask<string> GenerateRefreshTokenAsync(string accessToken)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        bool isCan = jwtSecurityTokenHandler.CanReadToken(accessToken);//验证Token格式
        if (!isCan) throw new Exception("传入访问令牌格式错误");

        //var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(refreshtoken);//转换类型为token，不用这一行

        TokenValidationParameters validateParameter = new()//验证参数
        {
            ValidateAudience = true,

            // 验证发布者
            ValidateIssuer = true,

            // 验证过期时间(過期會報異常)
            ValidateLifetime = true,

            // 验证秘钥
            ValidateIssuerSigningKey = true,

            // 读配置Issure
            ValidIssuer = tokenOption.Value.Issuer,

            // 读配置Audience
            ValidAudience = "testClient",

            // 设置生成token的秘钥
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.Value.SecretKey))
        };

        try
        {
            // Validation 1 - Validation JWT token format
            // 此验证功能将确保 Token 满足验证参数，并且它是一个真正的 token 而不仅仅是随机字符串
            //验证传入的过期的AccessToken
            //驗證傳入的過期 AccessToken
            var tokenInVerification = jwtSecurityTokenHandler.ValidateToken(accessToken, validateParameter, out var validatedToken);

            // Validation 2 - Validate encryption alg
            // 检查 token 是否有有效的安全算法
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (result == false)
                {
                    return null;
                }
            }

            // Validation 3 - validate expiry date
            // 验证原 token 的过期时间，得到 unix 时间戳
            var utcExpiryDate = long.Parse(tokenInVerification.Claims.First(item => item.Type is JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

            if (expiryDate > DateTime.UtcNow)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                {
                    "Token has not yet expired"
                }
                };
            }

            // validation 4 - validate existence of the token
            // 验证 refresh token 是否存在，是否是保存在数据库的 refresh token
            var storedRefreshToken = await _apiDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                {
                    "Refresh Token does not exist"
                }
                };
            }

            // Validation 5 - 检查存储的 RefreshToken 是否已过期
            // Check the date of the saved refresh token if it has expired
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Refresh Token has expired, user needs to re-login" },
                    Success = false
                };
            }

            // Validation 6 - validate if used
            // 验证 refresh token 是否已使用
            if (storedRefreshToken.IsUsed)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                {
                    "Refresh Token has been used"
                }
                };
            }

            // Validation 7 - validate if revoked
            // 检查 refresh token 是否被撤销
            if (storedRefreshToken.IsRevorked)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                {
                    "Refresh Token has been revoked"
                }
                };
            }

            // Validation 8 - validate the id
            // 这里获得原 JWT token Id
            var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            // 根据数据库中保存的 Id 验证收到的 token 的 Id
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthResult()
                {
                    Success = false,
                    Errors = new List<string>()
                {
                    "The token doesn't mateched the saved token"
                }
                };
            }

            // update current token 
            // 将该 refresh token 设置为已使用
            storedRefreshToken.IsUsed = true;
            _apiDbContext.RefreshTokens.Update(storedRefreshToken);
            await _apiDbContext.SaveChangesAsync();

            // 生成一个新的 token
            var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            return await GenerateJwtToken(dbUser);
        }
        catch (SecurityTokenException)
        {
            throw new Exception("传入AccessToken被修改");
        }
        //// 获取SecurityKey
        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.Value.SecretKey));
        //var jwtToken = validatedToken as JwtSecurityToken;//转换一下
        //var accClaims = jwtToken.Claims;
        //var access_Token = new JwtSecurityToken(
        //        issuer: "fcb",                    // 发布者
        //                                          //audience: "myClient",                // 接收者
        //        notBefore: DateTime.Now,                                                          // token签发时间
        //        expires: DateTime.Now.AddMinutes(30),                                             // token过期时间
        //        claims: accClaims,                                                                   // 该token内存储的自定义字段信息
        //        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)    // 用于签发token的秘钥算法
        //    );
        //// 返回成功信息，写出token
        //return jwtSecurityTokenHandler.WriteToken(access_Token);
        static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTimeVal;
        }
    }
    public void SetTokenContent(in TokenValidationParameters parameters) => TokenValidationParameters = parameters;

    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
    public TokenValidationParameters TokenValidationParameters { get; private set; }
}