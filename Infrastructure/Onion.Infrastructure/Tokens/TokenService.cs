using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Interfaces.Tokens;
using Onion.Domain.Entitites;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Onion.Infrastructure.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenSettings _tokenSettings;

        public TokenService(UserManager<User> userManager, IOptions<TokenSettings> options)
        {
            _tokenSettings = options.Value;
            _userManager = userManager;
        }

        public async Task<JwtSecurityToken> CreateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // bu token'a id değeri verecek
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // bu token'a kullanıcının id'sini verecek
                new Claim(JwtRegisteredClaimNames.Email, user.Email), //email bilgisini verecek
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                expires: DateTime.Now.AddMinutes(_tokenSettings.TokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            await _userManager.AddClaimsAsync(user, claims);
            // AspNetUserClaims tablosuna token alan her kullanıcının bilgilerini kaydetmek için ekledik
            // yani burada belirtilen alanları email bilgisi, rol bilgileri vs. bunları kaydetmek için


            return token;

        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)),
                ValidateLifetime = false,
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token Bulunamadı");
            }

            return principal;

        }
    }
}
