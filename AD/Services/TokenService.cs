using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using AD.Data;
    using AD.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly AuthDbContext _dbContext;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, AuthDbContext dbContext, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AuthResponse?> ValidateUserAndGenerateToken(AuthRequest request)
        {
            try
            {
                // Buscar usuario por nombre de usuario
                var user = await _dbContext.Users
                    .Include(u => u.Profile)
                    .ThenInclude(p => p.Permissions)
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                // Verificar si el usuario existe y la contraseña es correcta
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    _logger.LogWarning("Intento de autenticación fallido para el usuario: {Username}", request.Username);
                    return null;
                }

                // Verificar si el perfil del usuario está vigente
                if (user.Profile == null || user.Profile.ValidUntil < DateTime.Now)
                {
                    _logger.LogWarning("Perfil expirado o no existente para el usuario: {Username}", request.Username);
                    return null;
                }

                // Generar token JWT
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la validación del usuario");
                return null;
            }
        }

        private AuthResponse GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ??
                throw new InvalidOperationException("JWT Key not configured");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crear claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("employeeNumber", user.EmployeeNumber),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

            // Agregar permisos como claims
            var permissionsList = new List<string>();
            if (user.Profile?.Permissions != null)
            {
                foreach (var permission in user.Profile.Permissions)
                {
                    claims.Add(new Claim(ClaimTypes.Role, permission.RoleName));
                    permissionsList.Add(permission.RoleName);
                }
            }

            // Crear el token JWT
            var durationInMinutes = int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60");
            var expiration = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Username = user.Username,
                FullName = user.FullName,
                Permissions = permissionsList
            };
        }
    }
