using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SimpleShopApi.Utils
{
    public class JwtTokenGenerator(TokenValidationParameters parameters, int expiriesInMinutes)
    {
        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var signingCredentials = new SigningCredentials(
                parameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: parameters.ValidIssuer,
                audience: parameters.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiriesInMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string? GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, parameters, out _);
                return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
