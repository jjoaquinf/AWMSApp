using AD.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AD.Auth
{
    public class AuthRepository : IAuthRepository
    {
        public string Login(ClientCredentials credentials)
        {
            if (credentials.clientId == "miCliente" && credentials.clientSecret== "miSecretoSuperSeguro")
            {
                string token = GenerateJwtToken(credentials.clientId);
                return token;
            }
            return null;
        }

        private string GenerateJwtToken(string clientId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clave_super_secreta_para_firmar_jwt"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("client_id", clientId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: "mi_api",
                audience: "mi_api_clientes",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
