using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;

namespace AliceWebApp.Lib
{
    public class Authorization
    {
        
        private const string secretKey = "alice_wonderland!123";
        private static SymmetricSecurityKey signingKey = 
                                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials signingCredentials =
                                            new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        //---
        const string audience = "Wonderland";
        const string issuer = "Alice";
        //---
        public  TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = signingKey,

            ValidateIssuer = false,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero,
            AuthenticationType = JwtBearerDefaults.AuthenticationScheme
        };

        static DateTime now = DateTime.UtcNow;
        static Claim [] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, AuthedUser.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, "Wonderland"),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
        internal static object CreateToken()
        {
            var jwt = new JwtSecurityToken(
                    issuer: "Alice",
                    claims: claims,
                    notBefore: now,
                    expires: now.AddMinutes(45) 
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }

        public static readonly ApiUser AuthedUser = new ApiUser() { Name = "Alice", Password = "Wonderland" };

        public static bool IsAuthed(List<string> creds)
        {
            return creds.First().Equals(AuthedUser.Name.ToLower()) && creds.Last().Equals(AuthedUser.Password.ToLower());
        }
        
    }
}
