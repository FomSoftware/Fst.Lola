using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FomMonitoringCore.Service
{
    public class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "5WKenA5ZkB4K6pe8OBi0/KqaK9atKPAdBY6FqtZGC3axVibfwNruwj3+hOoIRqj8VHQXpepX9jThSrDwOveFMg==";

        public static string GenerateToken(string username, int expireMinutes = 2)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("data", now.AddDays(-5).ToString(), ClaimValueTypes.DateTime)
                }),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string stringToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(stringToken) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    LifetimeValidator =
                        (before, expires, token, parameters) =>
                        {
                            if (before.HasValue && before.Value > DateTime.UtcNow)
                                return false;

                            if (expires.HasValue)
                                return expires.Value > DateTime.UtcNow;

                            return true;
                        },
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(stringToken, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception e)
            {
                return null;
            }
        }
    }
}
