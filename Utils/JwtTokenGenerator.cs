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

        public Guid? GetUserIdFromToken(string token)
        {
            token = token.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, parameters, out _);
                var str = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (str == null) return null;
                return Guid.Parse(str);
            }
            catch
            {
                return null;
            }
        }
    }
}
