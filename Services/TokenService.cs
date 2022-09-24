using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using auth.Models;

namespace auth.Services
{
    public class TokenService
    {
        private string secretKey = "troi hom nay do con mua";
        public string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            var token = new JwtSecurityToken("NCD", "NCD",
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out SecurityToken validateToken);
            return (JwtSecurityToken)validateToken;
        }
        // public string Generate(string email, string password)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.ASCII.GetBytes(secretKey);
        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity(new Claim[]
        //         {
        //             new Claim(ClaimTypes.Name, email),
        //         }),
        //         Expires = DateTime.Now.AddHours(1),
        //         SigningCredentials = new SigningCredentials(
        //             new SymmetricSecurityKey(key),
        //             SecurityAlgorithms.HmacSha256Signature
        //         )
        //     };
        //     var token = tokenHandler.CreateToken(tokenDescriptor);
        //     return tokenHandler.WriteToken(token);
        // }
        // public string Generate(int id)
        // {
        //     var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //     var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        //     var header = new JwtHeader(credentials);

        //     var payload = new JwtPayload(id.ToString(), null, null, null, DateTime.Now.AddDays(1));
        //     var securityToken = new JwtSecurityToken(header, payload);
        //     return new JwtSecurityTokenHandler().WriteToken(securityToken);
        // }
        // public JwtSecurityToken Verify(string jwt)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.ASCII.GetBytes(secretKey);
        //     tokenHandler.ValidateToken(jwt, new TokenValidationParameters{
        //         IssuerSigningKey = new SymmetricSecurityKey(key),
        //         ValidateIssuerSigningKey = true,
        //         ValidateIssuer = false,
        //         ValidateAudience = false,
        //     }, out SecurityToken validateToken); 
        //     return (JwtSecurityToken)validateToken;
        // }
    }
}